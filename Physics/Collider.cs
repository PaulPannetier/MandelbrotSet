using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SME
{

    #region Line/Droite

    public class Line
    {
        public Vector2 A, B;
        public Line(in Vector2 A, in Vector2 B)
        {
            this.A = A;
            this.B = B;
        }

        public Line(in Vector2 start, in float angle, in float lenght)
        {
            A = start;
            B = new Vector2(A.X + lenght * MathF.Cos(angle), A.Y + lenght * MathF.Sin(angle));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(A, B, Color.Black);
        }
        public void Draw(SpriteBatch spriteBatch, in Color color)
        {
            spriteBatch.DrawLine(A, B, color);
        }
    }

    public class Droite
    {
        public Vector2 A, B;
        public Droite(in Vector2 A, in Vector2 B)
        {
            this.A = A;
            this.B = B;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawDroite(A, B);
        }
        public void Draw(SpriteBatch spriteBatch, in Color color)
        {
            spriteBatch.DrawDroite(A, B, color);
        }
    }

    #endregion

    public delegate void OnTriggerCollider(Sprite sprite, Collider collider);

    public class ColliderComparer : IComparer<Collider>
    {
        private static readonly Dictionary<Type, int> comparer = new Dictionary<Type, int>
        {
            { typeof(Circle), 10 },
            { typeof(Hitbox), 20 },
            { typeof(Capsule), 30 },
            { typeof(Polygone), 40 }
        };

        public int Compare(Collider c1, Collider c2)
        {
            if(c1 == null && c2 == null)
            {
                return 0;
            }
            if(c1  == null)
            {
                return -1;
            }
            if(c2 == null)
            {
                return 1;
            }
            return comparer[c1.GetType()] - comparer[c2.GetType()];
        }
    }

    public abstract class Collider
    {
        #region Collision Functions

        private static List<Vector2> cache = new List<Vector2>(), cache2 = new List<Vector2>(), cache3 = new List<Vector2>();

        public static bool CollideCircles(Circle c1, Circle c2) => c1.center.SqrDistance(c2.center) <= (c1.radius + c2.radius) * (c1.radius + c2.radius);
        public static bool CollideCircles(Circle c1, Circle c2, out Vector2 collisionPoint)
        {
            float sqrDist = c1.center.SqrDistance(c2.center);
            float r = (c1.radius + c2.radius) * (c1.radius + c2.radius);
            if (sqrDist <= r)// il y a collision
            {
                if(sqrDist < (c1.radius - c2.radius) * (c1.radius - c2.radius))//un cercle inclus dans l'autre
                {
                    if(c1.radius <= c2.radius)
                    {
                        float angle = Useful.Angle(c2.center - c1.center, Vector2.Zero);
                        collisionPoint = new Vector2(c2.center.X + c2.radius * MathF.Cos(angle), c2.center.Y + c2.radius * MathF.Sin(angle));
                    }
                    else
                    {
                        float angle = Useful.Angle(c1.center - c2.center, Vector2.Zero);
                        collisionPoint = new Vector2(c1.center.X + c1.radius * MathF.Cos(angle),  c1.center.Y + c1.radius * MathF.Sin(angle));
                    }
                    return true;
                }
                else//cas d'intersection normale
                {
                    if(MathF.Abs(c1.center.Y - c2.center.Y) < 1f)
                    {
                        float x = ((c2.radius * c2.radius) - (c1.radius * c1.radius) - (c2.center.X * c2.center.X) + (c1.center.X * c1.center.X)) / (2f * (c1.center.X - c2.center.X));
                        float b = -2f * c2.center.Y;
                        float c = (c2.center.X * c2.center.X) + (x * x) - (2f * c2.center.X * x) + (c2.center.Y * c2.center.Y) - (c2.radius * c2.radius);
                        float sqrtDelta = MathF.Sqrt((b * b) - (4f * c));
                        Vector2 i1 = new Vector2(x, (-b - sqrtDelta) / 2f);
                        Vector2 i2 = new Vector2(x, (-b + sqrtDelta) / 2f);
                        collisionPoint = (i1 + i2) / 2f;
                        return true;
                    }
                    else
                    {
                        float N = ((c2.radius * c2.radius) - (c1.radius * c1.radius) - (c2.center.X * c2.center.X) + (c1.center.X * c1.center.X) - (c2.center.Y * c2.center.Y) + (c1.center.Y * c1.center.Y)) / (2f * (c1.center.Y - c2.center.Y));
                        float temps = ((c1.center.X - c2.center.X) / (c1.center.Y - c2.center.Y));
                        float a = (temps * temps) + 1;
                        float b = (2f * c1.center.Y * temps) - (2f * N * temps) - (2f * c1.center.X);
                        float c = (c1.center.X * c1.center.X) + (c1.center.Y * c1.center.Y) + (N * N) - (c1.radius * c1.radius) - (2f * c1.center.Y * N);
                        float sqrtDelta = MathF.Sqrt((b * b) - (4f * a * c));
                        float x1 = (-b - sqrtDelta) / (2f * a);
                        float x2 = (-b + sqrtDelta) / (2f * a);
                        Vector2 i1 = new Vector2(x1, N - (x1 * temps));
                        Vector2 i2 = new Vector2(x2, N - (x2 * temps));
                        collisionPoint = (i1 + i2) / 2f;
                        return true;
                    }
                }
            }
            collisionPoint = Vector2.Zero;
            return false;
        }
        public static bool CollideCircles(Circle c1, Circle c2, out Vector2 collisionPoint, out Vector2 normal1, out Vector2 normal2)
        {
            if(CollideCircles(c1, c2, out collisionPoint))
            {
                normal1 = (collisionPoint - c1.center);
                normal1.Normalize();
                normal2 = (collisionPoint - c2.center);
                normal2.Normalize();
                return true;
            }
            normal1 = normal2 = Vector2.Zero;
            return false;
        }
        public static bool CollideCirclePolygone(Circle circle, Polygone polygone)
        {
            for (int i = 0; i < polygone.vertices.Count; i++)
            {
                if (circle.CollideLine(polygone.vertices[i], polygone.vertices[(i + 1) % polygone.vertices.Count]))
                    return true;
            }
            return polygone.Contains(circle.center);
        }
        public static bool CollideCirclePolygone(Circle circle, Polygone polygone, out Vector2 collisionPoint)
        {
            collisionPoint = Vector2.Zero;
            for (int i = 0; i < polygone.vertices.Count; i++)
            {
                if (CollideCircleLine(circle, polygone.vertices[i], polygone.vertices[(i + 1) % polygone.vertices.Count], out Vector2 intersection))
                {
                    cache.Add(intersection);
                }
            }
            if(cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();
                return true;
            }
            return polygone.Contains(circle.center);
        }
        public static bool CollideCirclePolygone(Circle circle, Polygone polygone, out Vector2 collisionPoint, out Vector2 normal1, out Vector2 normal2)
        {
            if (CollideCirclePolygone(circle, polygone, out collisionPoint))
            {
                normal1 = (collisionPoint - circle.center);
                normal1.Normalize();
                normal2 = -normal1;
                return true;
            }
            normal1 = normal2 = Vector2.Zero;
            return false;
        }
        public static bool CollideCircleHitbox(Circle circle, Hitbox hitbox) => CollideCirclePolygone(circle, hitbox.rec);
        public static bool CollideCircleHitbox(Circle circle, Hitbox hitbox, out Vector2 collisionPoint) => CollideCirclePolygone(circle, hitbox.rec, out collisionPoint);
        public static bool CollideCircleHitbox(Circle circle, Hitbox hitbox, out Vector2 collisionPoint, out Vector2 normal1, out Vector2 normal2) => CollideCirclePolygone(circle, hitbox.rec, out collisionPoint, out normal1, out normal2);
        public static bool CollideCircleLine(Circle c, in Vector2 A, in Vector2 B) => c.CollideLine(A, B);
        public static bool CollideCircleLine(Circle c, Line l) => c.CollideLine(l.A, l.B);
        public static bool CollideCircleLine(Circle c, in Vector2 A, in Vector2 B, out Vector2 collisionPoint)
        {
            if(!CollideCircleLine(c, A, B))
            {
                collisionPoint = Vector2.Zero;
                return false;
            }
            //on regarde si la droite est verticale
            if(MathF.Abs(A.X - B.X) < 1f)
            {
                float srqtDelta= MathF.Sqrt((c.radius * c.radius) - (((A.X + B.X) / 2f) - c.center.X) * (((A.X + B.X) / 2f) - c.center.X));
                Vector2 i1 = new Vector2(((A.X + B.X) / 2f), -srqtDelta + c.center.Y);
                Vector2 i2 = new Vector2(((A.X + B.X) / 2f), +srqtDelta + c.center.Y);
                //on verif que i1 et i2 appartienne au seg
                if(MathF.Min(A.Y, B.Y) <= i1.Y && MathF.Max(A.Y, B.Y) >= i1.Y && MathF.Min(A.Y, B.Y) <= i2.Y && MathF.Max(A.Y, B.Y) >= i2.Y)
                {
                    collisionPoint = (i1 + i2) / 2f;
                    return true;
                }
                if(MathF.Min(A.Y, B.Y) <= i1.Y && MathF.Max(A.Y, B.Y) >= i1.Y)
                {
                    collisionPoint = i1;
                    return true;
                }
                if (MathF.Min(A.Y, B.Y) <= i2.Y && MathF.Max(A.Y, B.Y) >= i2.Y)
                {
                    collisionPoint = i2;
                    return true;
                }
                collisionPoint = Vector2.Zero;
                return false;
            }
            else
            {
                float m = (B.Y - A.Y) / (B.X - A.X);
                float p = A.Y - m * A.X;
                float a = 1f + (m * m);
                float b = 2f * ((m * p) - c.center.X - (m * c.center.Y));
                float C = ((c.center.X * c.center.X) + (p * p) - (2f * p * c.center.Y) + (c.center.Y * c.center.Y) - (c.radius * c.radius));
                float sqrtDelta = MathF.Sqrt((b * b) - (4f * a * C));
                Vector2 i1 = new Vector2((-b - sqrtDelta) / (2f * a), m * ((-b - sqrtDelta) / (2f * a)) + p);
                Vector2 i2 = new Vector2((-b + sqrtDelta) / (2f * a), m * ((-b + sqrtDelta) / (2f * a)) + p);
                //on verif que i1 et i2 appartienne au seg
                if (MathF.Min(A.Y, B.Y) <= i1.Y && MathF.Max(A.Y, B.Y) >= i1.Y && MathF.Min(A.X, B.X) <= i1.X && MathF.Max(A.X, B.X) >= i1.X &&
                    MathF.Min(A.Y, B.Y) <= i2.Y && MathF.Max(A.Y, B.Y) >= i2.Y && MathF.Min(A.X, B.X) <= i2.X && MathF.Max(A.X, B.X) >= i2.X)
                {
                    collisionPoint = (i1 + i2) / 2f;
                    return true;
                }
                if (MathF.Min(A.Y, B.Y) <= i1.Y && MathF.Max(A.Y, B.Y) >= i1.Y && MathF.Min(A.X, B.X) <= i1.X && MathF.Max(A.X, B.X) >= i1.X)
                {
                    collisionPoint = i1;
                    return true;
                }
                if (MathF.Min(A.Y, B.Y) <= i2.Y && MathF.Max(A.Y, B.Y) >= i2.Y && MathF.Min(A.X, B.X) <= i2.X && MathF.Max(A.X, B.X) >= i2.X)
                {
                    collisionPoint = i2;
                    return true;
                }
                collisionPoint = Vector2.Zero;
                return false;
            }
        }
        public static bool CollideCircleLine(Circle c, in Vector2 A, in Vector2 B, out Vector2 collisionPoint, out Vector2 normal)
        {
            if(CollideCircleLine(c, A, B, out collisionPoint))
            {
                normal = (collisionPoint - c.center);
                normal.Normalize();
                return true;
            }
            normal = Vector2.Zero;
            return false;
        }
        public static bool CollideCircleLine(Circle c, Line l, out Vector2 collisionPoint, out Vector2 normal) => CollideCircleLine(c, l.A, l.B, out collisionPoint, out normal);
        public static bool CollideCircleLine(Circle c, Line l, out Vector2 collisionPoint) => CollideCircleLine(c, l.A, l.B, out collisionPoint);
        public static bool CollideCircleDroite(Circle c, Droite d) => c.CollideDroite(d);
        public static bool CollideCircleDroite(Circle c, in Vector2 A, in Vector2 B) => c.CollideDroite(A, B);
        public static bool CollideCircleDroite(Circle c, Droite d, out Vector2 collisionPoint) => CollideCircleDroite(c, d, out collisionPoint);
        public static bool CollideCircleDroite(Circle c, in Vector2 A, in Vector2 B, out Vector2 collisionPoint)
        {
            if (!CollideCircleDroite(c, A, B))
            {
                collisionPoint = Vector2.Zero;
                return false;
            }
            Vector2 u = new Vector2(B.X - A.X, B.Y - A.Y);
            Vector2 AC = new Vector2(c.center.X - A.X, c.center.Y - A.Y);
            float ti = (u.X * AC.X + u.Y * AC.Y) / (u.X * u.X + u.Y * u.Y);
            collisionPoint = new Vector2(A.X + ti * u.X, A.Y + ti * u.Y);
            return true;
        }
        public static bool CollideCircleDroite(Circle c, in Vector2 A, in Vector2 B, out Vector2 collisionPoint, out Vector2 normal)
        {
            if (CollideCircleDroite(c, A, B, out collisionPoint))
            {
                normal = (collisionPoint - c.center);
                normal.Normalize();
                return true;
            }
            normal = Vector2.Zero;
            return false;
        }
        public static bool CollideCircleDroite(Circle c, Droite d, out Vector2 collisionPoint, out Vector2 normal) => CollideCircleDroite(c, d.A, d.B, out collisionPoint, out normal);
        public static bool CollideCircleCapsule(Circle circle, Capsule caps)
        {
            return CollideCircleHitbox(circle, caps.hitbox) || CollideCircles(circle, caps.c1) || CollideCircles(circle, caps.c2);
        }
        public static bool CollideCircleCapsule(Circle circle, Capsule caps, out Vector2 collisionPoint)
        {
            if (CollideCircleHitbox(circle, caps.hitbox, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCircles(circle, caps.c2, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if(CollideCircles(circle, caps.c1, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            
            collisionPoint = Vector2.Zero;
            if (cache.Count > 0)
            {
                foreach  (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();
                return true;
            }
            return false;
        }
        public static bool CollideCircleCapsule(Circle circle, Capsule caps, out Vector2 collisionPoint, out Vector2 normal1, out Vector2 normal2)
        {
            if (CollideCircleCapsule(circle, caps, out collisionPoint))
            {
                normal1 = collisionPoint - circle.center;
                normal1.Normalize();
                normal2 = -normal1;
                return true;
            }
            normal1 = normal2 = Vector2.Zero;
            return false;
        }

        public static bool CollidePolygones(Polygone p1, Polygone p2)
        {
            for (int i = 0; i < p1.vertices.Count; i++)
            {
                for (int j = 0; j < p2.vertices.Count; j++)
                {
                    if (CollideLines(p1.vertices[i], p1.vertices[(i + 1) % p1.vertices.Count], p2.vertices[j], p2.vertices[(j + 1) % p2.vertices.Count]))
                    {
                        return true;
                    }
                }
            }
            return p1.Contains(p2.center) || p2.Contains(p1.center);
        }
        public static bool CollidePolygones(Polygone p1, Polygone p2, out Vector2 collisionPoint)
        {
            collisionPoint = Vector2.Zero;
            for (int i = 0; i < p1.vertices.Count; i++)
            {
                for (int j = 0; j < p2.vertices.Count; j++)
                {
                    if (CollideLines(p1.vertices[i], p1.vertices[(i + 1) % p1.vertices.Count], p2.vertices[j], p2.vertices[(j + 1) % p2.vertices.Count], out Vector2 intersec))
                    {
                        cache.Add(intersec);
                    }
                }
            }
            if(cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();
                return true;
            }
            return false;
        }
        public static bool CollidePolygones(Polygone p1, Polygone p2, out Vector2 collisionPoint, out Vector2 normal1, out Vector2 normal2)
        {
            for (int i = 0; i < p1.vertices.Count; i++)
            {
                for (int j = 0; j < p2.vertices.Count; j++)
                {
                    if (CollideLines(p1.vertices[i], p1.vertices[(i + 1) % p1.vertices.Count], p2.vertices[j], p2.vertices[(j + 1) % p2.vertices.Count], out Vector2 intersec))
                    {
                        cache.Add(intersec);
                        Vector2 n1 = (p1.vertices[(i + 1) % p1.vertices.Count] - p1.vertices[i]).NormalVector();
                        n1.Normalize();
                        //on regarde si on est dans le bon sens
                        Vector2 middle = (p1.vertices[i] + p1.vertices[(i + 1) % p1.vertices.Count]) / 2f;
                        if (p1.Contains(middle + n1))//tromper de sens
                        {
                            n1 *= -1f;
                        }
                        cache2.Add(n1);//Stocker le vecteur normal au coté de p1

                        Vector2 n2 = (p2.vertices[(j + 1) % p2.vertices.Count] - p2.vertices[j]).NormalVector();
                        n2.Normalize();
                        middle = (p2.vertices[j] + p2.vertices[(j + 1) % p2.vertices.Count]) / 2f;
                        if (p2.Contains(middle + n2))//tromper de sens
                        {
                            n2 *= -1f;
                        }
                        cache3.Add(n2);//Stocker le vecteur normal au coté de p2
                    }
                }
            }
            collisionPoint = normal1 = normal2 = Vector2.Zero;
            if (cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();

                float averageAngle = 0f, averageAngle2;
                foreach (Vector2 n in cache2)
                {
                    averageAngle += Useful.Angle(Vector2.Zero, n); 
                }
                averageAngle /= cache2.Count;
                averageAngle2 = Useful.WrapAngle(averageAngle + MathF.PI);
                float dist1 = 0f, dist2 = 0f;
                foreach (Vector2 n in cache2)
                {
                    float angle = Useful.Angle(Vector2.Zero, n);
                    dist1 += MathF.Abs(angle - averageAngle) % MathF.PI;
                    dist2 += MathF.Abs(angle - averageAngle2) % MathF.PI;
                }
                averageAngle = dist1 <= dist2 ? averageAngle : averageAngle2;
                normal1 = new Vector2(MathF.Cos(averageAngle), MathF.Sin(averageAngle));

                averageAngle = 0f;
                foreach (Vector2 n in cache3)
                {
                    averageAngle += Useful.Angle(Vector2.Zero, n);
                }
                averageAngle /= cache3.Count;
                averageAngle2 = Useful.WrapAngle(averageAngle + MathF.PI);
                dist1 = dist2 = 0f;
                foreach (Vector2 n in cache3)
                {
                    float angle = Useful.Angle(Vector2.Zero, n);
                    dist1 += MathF.Abs(angle - averageAngle) % MathF.PI;
                    dist2 += MathF.Abs(angle - averageAngle2) % MathF.PI;
                }
                averageAngle = dist1 <= dist2 ? averageAngle : averageAngle2;
                normal2 = new Vector2(MathF.Cos(averageAngle), MathF.Sin(averageAngle));

                cache2.Clear();
                cache3.Clear();
                return true;
            }
            return false;
        }
        public static bool CollidePolygoneHitbox(Polygone p, Hitbox h) => CollidePolygones(h.rec, p);
        public static bool CollidePolygoneHitbox(Polygone p, Hitbox h, out Vector2 collisionPoint) => CollidePolygones(p, h.rec, out collisionPoint);
        public static bool CollidePolygoneHitbox(Polygone p, Hitbox h, out Vector2 collisionPoint, out Vector2 normal1, out Vector2 normal2) => CollidePolygones(p, h.rec, out collisionPoint, out normal1, out normal2);
        public static bool CollidePolygoneLine(Polygone p, in Vector2 A, in Vector2 B) => p.CollideLine(A, B);
        public static bool CollidePolygoneLine(Polygone p, Line l) => p.CollideLine(l.A, l.B);
        public static bool CollidePolygoneLine(Polygone p, in Vector2 A, in Vector2 B, out Vector2 collisionPoint)
        {
            collisionPoint = Vector2.Zero;
            for (int i = 0; i < p.vertices.Count; i++)
            {
                if (CollideLines(p.vertices[i], p.vertices[(i + 1) % p.vertices.Count], A, B, out Vector2 intersec))
                {
                    cache.Add(intersec);
                }
            }
            if(cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();
                return true;
            }
            return false;
        }
        public static bool CollidePolygoneLine(Polygone p, Line l, out Vector2 collisionPoint) => CollidePolygoneLine(p, l.A, l.B, out collisionPoint);
        public static bool CollidePolygoneLine(Polygone p, in Vector2 A, in Vector2 B, out Vector2 collisionPoint, out Vector2 normal)
        {
            collisionPoint = Vector2.Zero;
            for (int i = 0; i < p.vertices.Count; i++)
            {
                if (CollideLines(p.vertices[i], p.vertices[(i + 1) % p.vertices.Count], A, B, out Vector2 intersec))
                {
                    cache.Add(intersec);
                    Vector2 n = (p.vertices[(i + 1) % p.vertices.Count] - p.vertices[i]).NormalVector();
                    n.Normalize();
                    //on regarde si on est dans le bon sens
                    Vector2 middle = (p.vertices[i] + p.vertices[(i + 1) % p.vertices.Count]) / 2f;
                    if (p.Contains(middle + n))//tromper de sens
                    {
                        n *= -1f;
                    }
                    cache2.Add(n);//Stocker le vecteur normal au coté de p1
                }
            }
            
            if (cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();
            
                if (cache2.Count == 1)
                {
                    normal = cache2[0];
                    cache2.Clear();
                    return true;
                }
                //on vérif le sens
                float averageAngle = 0f, averageAngle2;
                foreach (Vector2 n in cache2)
                {
                    averageAngle += Useful.Angle(Vector2.Zero, n);
                }
                averageAngle /= cache2.Count;
                averageAngle2 = Useful.WrapAngle(averageAngle + MathF.PI);
                float dist1 = 0f, dist2 = 0f;
                foreach (Vector2 n in cache2)
                {
                    float angle = Useful.Angle(Vector2.Zero, n);
                    dist1 += MathF.Abs(angle - averageAngle) % MathF.PI;
                    dist2 += MathF.Abs(angle - averageAngle2) % MathF.PI;
                }
                averageAngle = dist1 <= dist2 ? averageAngle : averageAngle2;
                normal = new Vector2(MathF.Cos(averageAngle), MathF.Sin(averageAngle));

                cache2.Clear();
                return true;
            }
            normal = Vector2.Zero;
            return false;
        }
        public static bool CollidePolygoneLine(Polygone p, Line l, out Vector2 collisionPoint, out Vector2 normal) => CollidePolygoneLine(p, l.A, l.B, out collisionPoint, out normal);
        public static bool CollidePolygoneDroite(Polygone p, in Vector2 A, in Vector2 B) => p.CollideDroite(A, B);
        public static bool CollidePolygoneDroite(Polygone p, Droite d) => p.CollideDroite(d);
        public static bool CollidePolygoneDroite(Polygone p, in Vector2 A, in Vector2 B, out Vector2 collisionPoint)
        {
            collisionPoint = Vector2.Zero;
            for (int i = 0; i < p.vertices.Count; i++)
            {
                if (CollideDroites(p.vertices[i], p.vertices[(i + 1) % p.vertices.Count], A, B, out Vector2 intersec))
                {
                    cache.Add(intersec);
                }
            }
            if (cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();
                return true;
            }
            return false;
        }
        public static bool CollidePolygoneDroite(Polygone p, Droite d, out Vector2 collisionPoint) => CollidePolygoneDroite(p, d.A, d.B, out collisionPoint);
        public static bool CollidePolygoneDroite(Polygone p, in Vector2 A, in Vector2 B, out Vector2 collisionPoint, out Vector2 normal)
        {
            collisionPoint = Vector2.Zero;
            for (int i = 0; i < p.vertices.Count; i++)
            {
                if (CollideLineDroite(p.vertices[i], p.vertices[(i + 1) % p.vertices.Count], A, B, out Vector2 intersec))
                {
                    cache.Add(intersec);
                    Vector2 n = (p.vertices[(i + 1) % p.vertices.Count] - p.vertices[i]).NormalVector();
                    n.Normalize();
                    //on regarde si on est dans le bon sens
                    Vector2 middle = (p.vertices[i] + p.vertices[(i + 1) % p.vertices.Count]) / 2f;
                    if (p.Contains(middle + n))//tromper de sens
                    {
                        n *= -1f;
                    }
                    cache2.Add(n);//Stocker le vecteur normal au coté de p1
                }
            }

            if (cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();

                if (cache2.Count == 1)
                {
                    normal = cache2[0];
                    cache2.Clear();
                    return true;
                }
                //on vérif le sens
                float averageAngle = 0f, averageAngle2;
                foreach (Vector2 n in cache2)
                {
                    averageAngle += Useful.Angle(Vector2.Zero, n);
                }
                averageAngle /= cache2.Count;
                averageAngle2 = Useful.WrapAngle(averageAngle + MathF.PI);
                float dist1 = 0f, dist2 = 0f;
                foreach (Vector2 n in cache2)
                {
                    float angle = Useful.Angle(Vector2.Zero, n);
                    dist1 += MathF.Abs(angle - averageAngle) % MathF.PI;
                    dist2 += MathF.Abs(angle - averageAngle2) % MathF.PI;
                }
                averageAngle = dist1 <= dist2 ? averageAngle : averageAngle2;
                normal = new Vector2(MathF.Cos(averageAngle), MathF.Sin(averageAngle));
                cache2.Clear();
                return true;
            }
            normal = Vector2.Zero;
            return false;
        }
        public static bool CollidePolygoneDroite(Polygone p, Droite d, out Vector2 collisionPoint, out Vector2 normal) => CollidePolygoneDroite(p, d.A, d.B, out collisionPoint, out normal);

        public static bool CollidePolygoneCapsule(Polygone p, Capsule caps) => CollidePolygoneHitbox(p, caps.hitbox) || CollideCirclePolygone(caps.c1, p) || CollideCirclePolygone(caps.c2, p);
        public static bool CollidePolygoneCapsule(Polygone p, Capsule caps, out Vector2 collisionPoint)
        {
            if(CollidePolygoneHitbox(p, caps.hitbox, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCirclePolygone(caps.c1, p, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCirclePolygone(caps.c2, p, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            collisionPoint = Vector2.Zero;
            if(cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();
                return true;
            }
            return false;
        }
        public static bool CollidePolygoneCapsule(Polygone p, Capsule caps, out Vector2 collisionPoint, out Vector2 normal1, out Vector2 normal2)
        {
            collisionPoint = normal1 = normal2 = Vector2.Zero;
            Vector2 col, n1, n2;
            if (CollideCirclePolygone(caps.c1, p, out col, out n1, out n2))
            {
                cache.Add(col);
                cache2.Add(n1);
                cache3.Add(n2);
            }
            if (CollideCirclePolygone(caps.c2, p, out col, out n1, out n2))
            {
                cache.Add(col);
                cache2.Add(n1);
                cache3.Add(n2);
            }
            if (CollidePolygoneHitbox(p, caps.hitbox, out col, out n1, out n2))
            {
                cache.Add(col);
                //cache2.Add(n1);
                //cache3.Add(n2);
                normal1 = n1;
                normal2 = n2;
            }
            if(cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();

                if (normal1 == Vector2.Zero)
                {
                    foreach (Vector2 n in cache2)
                    {
                        normal1 += n;
                    }
                    normal1 /= cache2.Count;
                    foreach (Vector2 n in cache3)
                    {
                        normal2 += n;
                    }
                    normal2 /= cache3.Count;
                }
                cache2.Clear();
                cache3.Clear();
                return true;
            }
            return false;
        }

        public static bool CollideHitboxs(Hitbox h1, Hitbox h2) => CollidePolygones(h1.rec, h2.rec);
        public static bool CollideHitboxs(Hitbox h1, Hitbox h2, out Vector2 collisionPoint) => CollidePolygones(h1.rec, h2.rec, out collisionPoint);
        public static bool CollideHitboxs(Hitbox h1, Hitbox h2, out Vector2 collisionPoint, out Vector2 normal1, out Vector2 normal2) => CollidePolygones(h1.rec, h2.rec, out collisionPoint, out normal1, out normal2);
        public static bool CollideHitboxLine(Hitbox h, in Vector2 A, in Vector2 B) => h.rec.CollideLine(A, B);
        public static bool CollideHitboxLine(Hitbox h, Line l) => h.rec.CollideLine(l.A, l.B);
        public static bool CollideHitboxLine(Hitbox h, in Vector2 A, in Vector2 B, out Vector2 collisionPoint) => CollidePolygoneLine(h.rec, A, B, out collisionPoint);
        public static bool CollideHitboxLine(Hitbox h, Line l, out Vector2 collisionPoint) => CollidePolygoneLine(h.rec, l.A, l.B, out collisionPoint);
        public static bool CollideHitboxLine(Hitbox h, in Vector2 A, in Vector2 B, out Vector2 collisionPoint, out Vector2 normal) => CollidePolygoneLine(h.rec, A, B, out collisionPoint, out normal);
        public static bool CollideHitboxLine(Hitbox h, Line l, out Vector2 collisionPoint, out Vector2 normal) => CollidePolygoneLine(h.rec, l.A, l.B, out collisionPoint, out normal);
        public static bool CollideHitboxDroite(Hitbox h, Droite d) => h.rec.CollideDroite(d);
        public static bool CollideHitboxDroite(Hitbox h, in Vector2 A, in Vector2 B) => h.rec.CollideDroite(A, B);
        public static bool CollideHitboxDroite(Hitbox h, in Vector2 A, in Vector2 B, out Vector2 collisionPoint) => CollidePolygoneDroite(h.rec, A, B, out collisionPoint);
        public static bool CollideHitboxDroite(Hitbox h, Line l, out Vector2 collisionPoint) => CollidePolygoneDroite(h.rec, l.A, l.B, out collisionPoint);
        public static bool CollideHitboxDroite(Hitbox h, in Vector2 A, in Vector2 B, out Vector2 collisionPoint, out Vector2 normal) => CollidePolygoneDroite(h.rec, A, B, out collisionPoint, out normal);
        public static bool CollideHitboxDroite(Hitbox h, Line l, out Vector2 collisionPoint, out Vector2 normal) => CollidePolygoneDroite(h.rec, l.A, l.B, out collisionPoint, out normal);
        public static bool CollideHitboxCapsule(Hitbox hitbox, Capsule caps) => CollideHitboxs(hitbox, caps.hitbox) || CollideCircleHitbox(caps.c1, hitbox) || CollideCircleHitbox(caps.c2, hitbox);
        public static bool CollideHitboxCapsule(Hitbox hitbox, Capsule caps, out Vector2 collisionPoint) => CollidePolygoneCapsule(hitbox.rec, caps, out collisionPoint);
        public static bool CollideHitboxCapsule(Hitbox hitbox, Capsule caps, out Vector2 collisionPoint, out Vector2 normal1, out Vector2 normal2) => CollidePolygoneCapsule(hitbox.rec, caps, out collisionPoint, out normal1, out normal2);

        public static bool CollideCapsules(Capsule caps1, Capsule caps2) => CollideHitboxCapsule(caps1.hitbox, caps2) || CollideCircleCapsule(caps1.c1, caps2) || CollideCircleCapsule(caps1.c2, caps2);
        public static bool CollideCapsules(Capsule caps1, Capsule caps2, out Vector2 collisionPoint)
        {
            if(CollideHitboxs(caps1.hitbox, caps2.hitbox, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCircleHitbox(caps2.c1, caps1.hitbox, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCircleHitbox(caps2.c2, caps1.hitbox, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCircleHitbox(caps1.c1, caps2.hitbox, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCircles(caps1.c1, caps2.c1, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCircles(caps1.c1, caps2.c2, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCircleHitbox(caps1.c2, caps2.hitbox, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCircles(caps1.c2, caps2.c1, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCircles(caps1.c2, caps2.c2, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            collisionPoint = Vector2.Zero;
            if(cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();
                return true;
            }
            return false;
        }
        public static bool CollideCapsules(Capsule caps1, Capsule caps2, out Vector2 collisionPoint, out Vector2 normal1, out Vector2 normal2)
        {
            if(CollideCapsules(caps1, caps2, out collisionPoint))
            {
                normal1 = collisionPoint - caps1.center;
                normal1.Normalize();
                normal2 = collisionPoint - caps2.center;
                normal2.Normalize();
                return true;
            }
            normal1 = normal2 = Vector2.Zero;
            return false;
        }
        public static bool CollideCapsuleLine(Capsule caps, Line l) => caps.CollideLine(l);
        public static bool CollideCapsuleLine(Capsule caps, in Vector2 A, in Vector2 B) => caps.CollideLine(A, B);
        public static bool CollideCapsuleLine(Capsule caps, in Vector2 A, in Vector2 B, out Vector2 collisionPoint)
        {
            if(CollideCircleLine(caps.c1, A, B, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCircleLine(caps.c2, A, B, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideHitboxLine(caps.hitbox, A, B, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            collisionPoint = Vector2.Zero;
            if(cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();
                return true;
            }
            return false;
        }
        public static bool CollideCapsuleLine(Capsule caps, Line l, out Vector2 collisionPoint) => CollideCapsuleLine(caps, l.A, l.B, out collisionPoint);
        public static bool CollideCapsuleLine(Capsule caps, in Vector2 A, in Vector2 B, out Vector2 collisionPoint, out Vector2 normal)
        {
            if(CollideCapsuleLine(caps, A, B, out collisionPoint))
            {
                normal = collisionPoint - caps.center;
                normal.Normalize();
                return true;
            }
            normal = Vector2.Zero;
            return false;
        }
        public static bool CollideCapsuleLine(Capsule caps, Line l, out Vector2 collisionPoint, out Vector2 normal) => CollideCapsuleLine(caps, l.A, l.B, out collisionPoint, out normal);
        public static bool CollideCapsuleDroite(Capsule caps, Droite d) => caps.CollideDroite(d);
        public static bool CollideCapsuleDroite(Capsule caps, in Vector2 A, in Vector2 B) => caps.CollideDroite(A, B);
        public static bool CollideCapsuleDroite(Capsule caps, in Vector2 A, in Vector2 B, out Vector2 collisionPoint)
        {
            if (CollideCircleDroite(caps.c1, A, B, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideCircleDroite(caps.c2, A, B, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            if (CollideHitboxDroite(caps.hitbox, A, B, out collisionPoint))
            {
                cache.Add(collisionPoint);
            }
            collisionPoint = Vector2.Zero;
            if (cache.Count > 0)
            {
                foreach (Vector2 pos in cache)
                {
                    collisionPoint += pos;
                }
                collisionPoint /= cache.Count;
                cache.Clear();
                return true;
            }
            return false;
        }
        public static bool CollideCapsuleDroite(Capsule caps, Line l, out Vector2 collisionPoint) => CollideCapsuleDroite(caps, l.A, l.B, out collisionPoint);
        public static bool CollideCapsuleDroite(Capsule caps, in Vector2 A, in Vector2 B, out Vector2 collisionPoint, out Vector2 normal)
        {
            if (CollideCapsuleDroite(caps, A, B, out collisionPoint))
            {
                normal = collisionPoint - caps.center;
                normal.Normalize();
                return true;
            }
            normal = Vector2.Zero;
            return false;
        }
        public static bool CollideCapsuleDroite(Capsule caps, Line l, out Vector2 collisionPoint, out Vector2 normal) => CollideCapsuleDroite(caps, l.A, l.B, out collisionPoint, out normal);

        #region Collide Lines/Droites   

        public static bool CollideDroites(Droite d1, Droite d2) => CollideDroites(d1.A, d1.B, d2.A, d2.B);
        public static bool CollideDroites(Droite d1, Droite d2, out Vector2 collisionPoint) => CollideDroites(d1.A, d1.B, d2.A, d2.B, out collisionPoint);
        public static bool CollideDroites(in Vector2 A, in Vector2 B, in Vector2 O, in Vector2 P)
        {
            return !(B - A).IsCollinear(P - O);
        }
        public static bool CollideDroites(in Vector2 A, in Vector2 B, in Vector2 O, in Vector2 P, out Vector2 collisionPoint)
        {
            //on regarde si une des droites est verticale
            if (Math.Abs(B.X - A.X) < 1f || Math.Abs(P.X - O.X) < 1f)
            {
                //si les 2 sont verticales
                if (Math.Abs(B.X - A.X) < 1f && Math.Abs(P.X - O.X) < 1f)
                {
                    if (Math.Abs(((A.X + B.X) / 2f) - ((O.X + P.X) / 2f)) >= 1f)
                    {
                        collisionPoint = Vector2.Zero;
                        return false;
                    }
                    collisionPoint = new Vector2((O.X + P.X + A.X + B.X) / 4f, (O.Y + P.Y + A.Y + B.Y) / 4f);
                    return true;
                }
                float a, b, ySol;
                if (Math.Abs(B.X - A.X) < 1f)//(AB) verticale mais pas (OP)
                {
                    a = (P.Y - O.Y) / (P.X - O.X);
                    b = O.Y - a * O.X;
                    ySol = a * ((A.X + B.X) / 2f) + b;
                    collisionPoint = new Vector2((A.X + B.X) / 2f, ySol);
                    return true;
                }
                // on sait que (OP) verticale mais pas (AB)
                a = (B.Y - A.Y) / (B.X - A.X);
                b = A.Y - a * A.X;
                ySol = a * ((O.X + P.X) / 2f) + b;
                collisionPoint = new Vector2((O.X + P.X) / 2f, ySol);
                return true;
            }

            //on regarde si une des droites est horizontale
            if (MathF.Abs(A.Y - B.Y) < 1f || MathF.Abs(O.Y - P.Y) < 1f)
            {
                if (MathF.Abs(A.Y - B.Y) < 1f && MathF.Abs(O.Y - P.Y) < 1f)//les 2 droites sont horizontales
                {
                    if (MathF.Abs(((A.Y + B.Y) / 2f) - ((O.Y + P.Y) / 2f)) < 1f)
                    {
                        collisionPoint = new Vector2((A.X + B.X + O.X + P.X) / 4f, (A.Y + B.Y + O.Y + P.Y) / 4f);
                        return true;
                    }
                    collisionPoint = Vector2.Zero;
                    return false;
                }
                float a, b, xSol;
                if (MathF.Abs(A.Y - B.Y) < 1f)//droite AB horizontal, seg OP non horizontal
                {
                    a = (P.Y - O.Y) / (P.X - O.X);
                    b = O.Y - a * O.X;
                    xSol = (((A.Y + B.Y) / 2f) - b) / a;
                    collisionPoint = new Vector2(xSol, (A.Y + B.Y) / 2f);
                    return true;
                }
                //OP horizontale
                a = (B.Y - A.Y) / (B.X - A.X);
                b = A.Y - a * A.X;
                xSol = (((O.Y + P.Y) / 2f) - b) / a;
                collisionPoint = new Vector2(xSol, (O.Y + P.Y) / 2f);
                return true;
            }

            //les 2 droites sont quelconques (pas horizontales ni verticales)
            float a1, b1, a2, b2;
            //equetion de la droite (A,B)
            a1 = (B.Y - A.Y) / (B.X - A.X);
            b1 = A.Y - a1 * A.X;
            //equation du droite (O,P)
            a2 = (P.Y - O.Y) / (P.X - O.X);
            b2 = O.Y - a2 * O.X;
            //On regarde si les 2 sont !//
            if (MathF.Abs(a1 - a2) >= 0.0025f)
            {
                float xSol = (b2 - b1) / (a1 - a2);
                float ySol = ((a1 * xSol) + b1 + (a2 * xSol) + b2) / 2f;
                collisionPoint = new Vector2(xSol, ySol);
                return true;
            }
            else if (MathF.Abs(b2 - b1) < 1f)
            {
                collisionPoint = new Vector2((A.X + B.X + O.X + P.X) / 4f, ((a1 + a2) / 2f) * ((A.X + B.X + O.X + P.X) / 4f) + ((b1 + b2) / 2f));
                return true;
            }
            collisionPoint = Vector2.Zero;
            return false;
        }
        public static bool CollideLines(Line l1, Line l2) => CollideLines(l1.A, l1.B, l2.A, l2.B);
        public static bool CollideLines(Line l1, Line l2, out Vector2 collisionPoint) => CollideLines(l1.A, l1.B, l2.A, l2.B, out collisionPoint);
        public static bool CollideLines(in Vector2 A, in Vector2 B, in Vector2 O, in Vector2 P)
        {
            return CollideLineDroite(A, B, O, P) && CollideLineDroite(O, P, A, B);
        }
        public static bool CollideLines(in Vector2 A, in Vector2 B, in Vector2 O, in Vector2 P, out Vector2 collisionPoint)
        {
            //on regarde si un des 2 segments est vertical
            if (Math.Abs(B.X - A.X) < 1f || Math.Abs(P.X - O.X) < 1f)
            {
                //si les 2 sont verticals
                if (Math.Abs(B.X - A.X) < 1f && Math.Abs(P.X - O.X) < 1f)
                {
                    if (Math.Abs(A.X - O.X) >= 1f)
                    {
                        collisionPoint = Vector2.Zero;
                        return false;
                    }
                    float minDesMax = Math.Min(Math.Max(A.Y, B.Y), Math.Max(O.Y, P.Y));
                    float maxDesMin = Math.Max(Math.Min(A.Y, B.Y), Math.Min(O.Y, P.Y));
                    if(minDesMax >= maxDesMin)
                    {
                        collisionPoint = new Vector2((A.X + B.X + O.X + P.X) / 4f, (maxDesMin + minDesMax) / 2f);
                        return true;
                    }
                    collisionPoint = Vector2.Zero;
                    return false;
                }
                float a, b, ySol;
                if (Math.Abs(B.X - A.X) < 1f)//AB vertical mais pas OP
                {
                    if (!(((A.X + B.X) / 2f) >= Math.Min(O.X, P.X) && ((A.X + B.X) / 2f) <= Math.Max(O.X, P.X)))
                    {
                        collisionPoint = Vector2.Zero;
                        return false;
                    }
                    a = (P.Y - O.Y) / (P.X - O.X);
                    b = O.Y - a * O.X;
                    ySol = a * ((A.X + B.X) / 2f) + b;
                    if(Math.Min(A.Y, B.Y) <= ySol && Math.Max(A.Y, B.Y) >= ySol)
                    {
                        collisionPoint = new Vector2((A.X + B.X) / 2f, ySol);
                        return true;
                    }
                    collisionPoint = Vector2.Zero;
                    return false;
                }
                // on sait que [OP] vertical
                if (!(((O.X + P.X) / 2f) >= Math.Min(A.X, B.X) && ((O.X + P.X) / 2f) <= Math.Max(A.X, B.X)))
                {
                    collisionPoint = Vector2.Zero;
                    return false;
                }
                a = (B.Y - A.Y) / (B.X - A.X);
                b = A.Y - a * A.X;
                ySol = a * ((O.X + P.X) / 2f) + b;
                if(Math.Min(O.Y, P.Y) <= ySol && Math.Max(O.Y, P.Y) >= ySol)
                {
                    collisionPoint = new Vector2((O.X + P.X) / 2f, ySol);
                    return true;
                }
                collisionPoint = Vector2.Zero;
                return false;
            }
            //on regarde si un des 2 segment est horizontale
            if(MathF.Abs(A.Y - B.Y) < 1f || MathF.Abs(O.Y - P.Y) < 1f)
            {
                if(MathF.Abs(A.Y - B.Y) < 1f && MathF.Abs(O.Y - P.Y) < 1f)//les 2 segments sont horizontaux
                {
                    if(MathF.Abs(((A.Y + B.Y)/2f) - ((O.Y + P.Y)/2f)) < 1f)
                    {
                        float minDesMax = Math.Min(Math.Max(A.X, B.X), Math.Max(O.X, P.X));
                        float maxDesMin = Math.Max(Math.Min(A.X, B.X), Math.Min(O.X, P.X));
                        if(minDesMax >= maxDesMin)//si il y a collision
                        {
                            collisionPoint = new Vector2((maxDesMin + minDesMax) / 2f, (A.Y + B.Y + O.Y + P.Y) / 4f);
                            return true;
                        }
                    }
                    collisionPoint = Vector2.Zero;
                    return false;
                }
                float a, b, xSol;
                if(MathF.Abs(A.Y - B.Y) < 1f)//AB horizontal, OP non horizontal
                {
                    a = (P.Y - O.Y) / (P.X - O.X);
                    b = O.Y - a * O.X;
                    xSol = (((A.Y + B.Y) / 2f) - b) / a;
                    if(MathF.Min(A.X, B.X) <= xSol && MathF.Max(A.X, B.X) >= xSol && MathF.Min(O.X, P.X) <= xSol && MathF.Max(O.X, P.X) >= xSol)
                    {
                        collisionPoint = new Vector2(xSol, (A.Y + B.Y) / 2f);
                        return true;
                    }
                    collisionPoint = Vector2.Zero;
                    return false;
                }
                //OP horizontale
                a = (B.Y - A.Y) / (B.X - A.X);
                b = A.Y - a * A.X;
                xSol = (((O.Y + P.Y) / 2f) - b) / a;
                if (MathF.Min(O.X, P.X) <= xSol && MathF.Max(O.X, P.X) >= xSol && MathF.Min(A.X, B.X) <= xSol && MathF.Max(A.X, B.X) >= xSol)
                {
                    collisionPoint = new Vector2(xSol, (O.Y + P.Y) / 2f);
                    return true;
                }
                collisionPoint = Vector2.Zero;
                return false;
            }

            //les 2 segments sont quelconques (pas horizontaux ni verticaux)
            float a1, b1, a2, b2;
            //equetion de la droite (A,B)
            a1 = (B.Y - A.Y) / (B.X - A.X);
            b1 = A.Y - a1 * A.X;
            //equation de la droite (O,P)
            a2 = (P.Y - O.Y) / (P.X - O.X);
            b2 = O.Y - a2 * O.X;
            //On regarde si les 2 segment sont !//
            if (MathF.Abs(a1 - a2) >= 0.001f)
            {
                float xSol = (b2 - b1) / (a1 - a2);
                float ySol = ((a1 * xSol) + b1 + (a2 * xSol) + b2) / 2f;
                if (MathF.Min(A.X, B.X) <= xSol && MathF.Max(A.X, B.X) >= xSol && MathF.Min(A.Y, B.Y) <= ySol && MathF.Max(A.Y, B.Y) >= ySol
                    && MathF.Min(O.X, P.X) <= xSol && MathF.Max(O.X, P.X) >= xSol && MathF.Min(O.Y, P.Y) <= ySol && MathF.Max(O.Y, P.Y) >= ySol)
                {
                    collisionPoint = new Vector2(xSol, ySol);
                    return true;
                }
                collisionPoint = Vector2.Zero;
                return false;
            }
            else if(MathF.Abs(b2 - b1) < 1f)
            {
                collisionPoint = new Vector2((A.X + B.X + O.X + P.X) / 4f, ((a1 + a2) / 2f) * ((A.X + B.X + O.X + P.X) / 4f) + ((b1 + b2) / 2f));
                return true;
            }
            collisionPoint = Vector2.Zero;
            return false;
        }
        public static bool CollideLines(float angle1, float angle2, in Vector2 p1, in Vector2 p2, in float length1, in float length2)
        {
            Useful.WrapAngle(angle1);
            Useful.WrapAngle(angle2);
            return CollideLines(p1, new Vector2(p1.X + length1 * (float)Math.Cos(angle1), p1.Y + length1 * (float)Math.Sin(angle1)),
                p2, new Vector2(p2.X + length2 * (float)Math.Cos(angle2), p2.Y + length2 * (float)Math.Sin(angle2)));
        }
        public static bool CollideLines(float angle1, float angle2, in Vector2 p1, in Vector2 p2, in float length1, in float length2, out Vector2 collisionPoint)
        {
            Useful.WrapAngle(angle1);
            Useful.WrapAngle(angle2);
            return CollideLines(p1, new Vector2(p1.X + length1 * (float)Math.Cos(angle1), p1.Y + length1 * (float)Math.Sin(angle1)),
                p2, new Vector2(p2.X + length2 * (float)Math.Cos(angle2), p2.Y + length2 * (float)Math.Sin(angle2)), out collisionPoint);
        }
        public static bool CollideLineDroite(Line l, Droite d) => CollideLineDroite(l.A, l.B, d.A, d.B);
        public static bool CollideLineDroite(Line l, Droite d, out Vector2 collisionPoint) => CollideLineDroite(l.A, l.B, d.A, d.B, out collisionPoint);
        public static bool CollideLineDroite(in Vector2 O, in Vector2 P, in Vector2 A, in Vector2 B)
        {
            Vector2 AB = B - A;
            Vector2 AP = P - A;
            Vector2 AO = O - A;
            return (AB.X * AP.Y - AB.Y * AP.X) * (AB.X * AO.Y - AB.Y * AO.X) < 0;
        }
        public static bool CollideLineDroite(in Vector2 O, in Vector2 P, in Vector2 A, in Vector2 B, out Vector2 collisionPoint)
        {
            //on regarde si le segment ou la droite est vertical
            if (Math.Abs(B.X - A.X) < 1f || Math.Abs(P.X - O.X) < 1f)
            {
                //si les 2 sont verticals
                if (Math.Abs(B.X - A.X) < 1f && Math.Abs(P.X - O.X) < 1f)
                {
                    if (Math.Abs(A.X - O.X) >= 1f)
                    {
                        collisionPoint = Vector2.Zero;
                        return false;
                    }
                    collisionPoint = new Vector2((O.X + P.X + A.X + B.X) / 4f, MathF.Min(O.Y, P.Y) + MathF.Abs(O.Y - P.Y) / 2f);
                    return true;
                }
                float a, b, ySol;
                if (Math.Abs(B.X - A.X) < 1f)//AB vertical mais pas OP
                {
                    if (!(((A.X + B.X) / 2f) >= Math.Min(O.X, P.X) && ((A.X + B.X) / 2f) <= Math.Max(O.X, P.X)))
                    {
                        collisionPoint = Vector2.Zero;
                        return false;
                    }

                    a = (P.Y - O.Y) / (P.X - O.X);
                    b = O.Y - a * O.X;
                    ySol = a * ((A.X + B.X) / 2f) + b;
                    collisionPoint = new Vector2((A.X + B.X) / 2f, ySol);
                    return true;
                }
                // on sait que [OP] vertical

                a = (B.Y - A.Y) / (B.X - A.X);
                b = A.Y - a * A.X;
                ySol = a * ((O.X + P.X) / 2f) + b;
                if (MathF.Min(O.Y, P.Y) <= ySol && MathF.Max(O.Y, P.Y) >= ySol)
                {
                    collisionPoint = new Vector2((O.X + P.X) / 2f, ySol);
                    return true;
                }
                collisionPoint = Vector2.Zero;
                return false;
            }

            //on regarde si le seg ou la droite est horizontale
            if (MathF.Abs(A.Y - B.Y) < 1f || MathF.Abs(O.Y - P.Y) < 1f)
            {
                if (MathF.Abs(A.Y - B.Y) < 1f && MathF.Abs(O.Y - P.Y) < 1f)//le segment et la droite sont horizontaux
                {
                    if (MathF.Abs(((A.Y + B.Y) / 2f) - ((O.Y + P.Y) / 2f)) < 1f)
                    {
                        collisionPoint = new Vector2((O.X + P.X) / 2f, (A.Y + B.Y + O.Y + P.Y) / 4f);
                        return true;
                    }
                    collisionPoint = Vector2.Zero;
                    return false;
                }
                float a, b, xSol;
                if (MathF.Abs(A.Y - B.Y) < 1f)//droite AB horizontal, seg OP non horizontal
                {
                    a = (P.Y - O.Y) / (P.X - O.X);
                    b = O.Y - a * O.X;
                    xSol = (((A.Y + B.Y) / 2f) - b) / a;
                    if (MathF.Min(O.X, P.X) <= xSol && MathF.Max(O.X, P.X) >= xSol)
                    {
                        collisionPoint = new Vector2(xSol, (A.Y + B.Y) / 2f);
                        return true;
                    }
                    collisionPoint = Vector2.Zero;
                    return false;
                }
                //OP horizontale
                a = (B.Y - A.Y) / (B.X - A.X);
                b = A.Y - a * A.X;
                xSol = (((O.Y + P.Y) / 2f) - b) / a;
                if (MathF.Min(O.X, P.X) <= xSol && MathF.Max(O.X, P.X) >= xSol)
                {
                    collisionPoint = new Vector2(xSol, (O.Y + P.Y) / 2f);
                    return true;
                }
                collisionPoint = Vector2.Zero;
                return false;
            }

            //le segment et la droite sont quelconque (pas horizontal ni vertical)
            float a1, b1, a2, b2;
            //equetion de la droite (A,B)
            a1 = (B.Y - A.Y) / (B.X - A.X);
            b1 = A.Y - a1 * A.X;
            //equation du segment [O,P]
            a2 = (P.Y - O.Y) / (P.X - O.X);
            b2 = O.Y - a2 * O.X;
            //On regarde si les 2 sont !//
            if (MathF.Abs(a1 - a2) >= 0.001f)
            {
                float xSol = (b2 - b1) / (a1 - a2);
                float ySol = ((a1 * xSol) + b1 + (a2 * xSol) + b2) / 2f;
                if (MathF.Min(O.X, P.X) <= xSol && MathF.Max(O.X, P.X) >= xSol && MathF.Min(O.Y, P.Y) <= ySol && MathF.Max(O.Y, P.Y) >= ySol)
                {
                    collisionPoint = new Vector2(xSol, ySol);
                    return true;
                }
                collisionPoint = Vector2.Zero;
                return false;
            }
            else if (MathF.Abs(b2 - b1) < 1f)
            {
                collisionPoint = new Vector2((O.X + P.X) / 2f, ((a1 + a2) / 2f) * ((O.X + P.X) / 2f) + ((b1 + b2) / 2f));
                return true;
            }
            collisionPoint = Vector2.Zero;
            return false;
        }

        #endregion

        #endregion

        private bool _isTrigger;
        public bool isTrigger
        {
            get => _isTrigger;
            set
            {
                _isTrigger = value;
                if(value)
                {
                    if(oldSpriteCollider == null)
                    {
                        oldSpriteCollider = new List<Collider>();
                        newSpriteCollider = new List<Collider>();
                    }
                    if (sprite != null)
                        sprite.ChangeToTriggerCollider(this);
               }
            }
        }
        public OnTriggerCollider OnTriggerEnter, OnTriggerExit, OnTriggerStay;
        public List<Collider> oldSpriteCollider, newSpriteCollider;

        public Sprite sprite;//le sprite attaché au collider
        public string layerMask = "none";
        public Vector2 defaultOffset;//Décalage par rapport a la position du sprite
        public virtual Color color { get; set; }
        public virtual float thickness { get; set; } = 1;
        public virtual Vector2 center
        {
            get => new Vector2();
            protected set { }
        }
        public virtual Point size
        {
            get => new Point();
            protected set { }
        }
        public ColliderMaterial material = null;

        public Collider(Sprite sprite)
        {
            this.sprite = sprite;
            defaultOffset = Vector2.Zero;
        }
        public Collider(Sprite sprite, in Vector2 defaultOffset)
        {
            this.sprite = sprite;
            this.defaultOffset = defaultOffset;
        }
        public virtual Collider Clone() => null;
        public bool CompareMask(string mask) => mask == layerMask;
        public bool CompareMask(List<string> masks) => masks.Contains(layerMask);
        public virtual bool Collide(Collider c) => false;
        public virtual bool CollideLine(Line l) => false;
        public virtual bool CollideLine(in Vector2 A, in Vector2 B) => false;
        public virtual bool CollideDroite(Droite d) => false;
        public virtual bool CollideDroite(in Vector2 A, in Vector2 B) => false;
        public virtual bool Contains(Vector2 p) => false;
        public virtual bool Contains(Point p) => false;
        public virtual void Move(Vector2 shift) { }
        public virtual void Move(Point shift) { }
        public virtual void Move(in float vx, in float vy) { }
        public virtual void MoveAt(Vector2 shift) { }
        public virtual void MoveAt(Point shift) { }
        public virtual void MoveAt(in float vx, in float vy) { }
        public virtual void Rotate(in float angle)
        {
            if (defaultOffset != Vector2.Zero)
            {
                float norme = defaultOffset.Length();
                float angTotal = Useful.Angle(Vector2.Zero, defaultOffset) + angle;
                defaultOffset = new Vector2((float)(norme * Math.Cos(angTotal)), (float)(norme * Math.Sin(angTotal)));
            }
        }
        public virtual Hitbox ToHitbox() => null;
        public virtual Circle ToInclusiveCircle() => null;//Doit etre large, osef si la précision est pas ouf
        public virtual void SetScale(in Vector2 newScale, in Vector2 oldScale) { }
        public virtual void Draw(SpriteBatch spriteBacth) {  }
        public void SetTrigger(OnTriggerCollider OnTriggerEnter, OnTriggerCollider OnTriggerExit, OnTriggerCollider OnTriggerStay)
        {
            this.OnTriggerEnter = OnTriggerEnter;
            this.OnTriggerExit = OnTriggerExit;
            this.OnTriggerStay = OnTriggerStay;
        }
        public void SetMaterial(ColliderMaterial material)
        { 
            this.material = material;
        }
    }

    public class Polygone : Collider
    {
        public List<Vector2> vertices;
        public Vector2 _center;

        public override Vector2 center
        {
            get => _center;
            protected set
            {
                _center = value;
            }
        }
        public override Point size
        {
            get => ToHitbox().size;
            protected set { }
        }

        #region Builder

        public Polygone(Sprite sprite, List<Vector2> vertices) : base(sprite)
        {
            center = Vector2.Zero;
            foreach (Vector2 pos in vertices)
            {
                center += pos;
            }
            center /= vertices.Count;
            Builder(vertices, center, Vector2.Zero, Color.Black, 1);
        }
        public Polygone(Sprite sprite, List<Vector2> vertices, in Vector2 center) : base(sprite)
        {
            Builder(vertices, center, Vector2.Zero, Color.Black, 1);
        }
        public Polygone(Sprite sprite, List<Vector2> vertices, in Vector2 center, in Vector2 defaultOffset) : base(sprite)
        {
            Builder(vertices, center, defaultOffset, Color.Black, 1);
        }
        public Polygone(Sprite sprite, List<Vector2> vertices, in Vector2 center, in Vector2 defaultOffset, in Color color, in float thickness) : base(sprite)
        {
            Builder(vertices, center, defaultOffset, color, thickness);
        }
        private void Builder(List<Vector2> vertices, in Vector2 center, in Vector2 defaultOffset, in Color color, in float thickness)
        {
            this.vertices = vertices; this.center = center; this.defaultOffset = defaultOffset; this.color = color; this.thickness = thickness; 
        }

        public override Collider Clone() => new Polygone(sprite, vertices.Clone(), center, defaultOffset, color, thickness);

        #endregion

        public override bool Collide(Collider c)
        {
            if(c is Hitbox)
            {
                return CollidePolygoneHitbox(this, (Hitbox)c);
            }
            else if(c is Circle)
            {
                return CollideCirclePolygone((Circle)c, this); 
            }
            else if(c is Polygone)
            {
                return CollidePolygones((Polygone)c, this);
            }
            else // capsule
            {
                return CollidePolygoneCapsule(this, (Capsule)c);
            }
        }

        #region Contain

        public override bool Contains(Point p) => Contains(p.ToVector2());
        public override bool Contains(Vector2 P)
        {
            int i;
            Vector2 I = new Vector2(100000f + Random.Rand(0, 9999), 100000f + Random.Rand(0, 9999));
            int nbintersections = 0;
            for (i = 0; i < vertices.Count; i++)
            {
                Vector2 A = vertices[i];
                Vector2 B = vertices[(i + 1) % vertices.Count];
                int iseg = intersectsegment(A, B, I, P);
                if (iseg == -1)
                    return Contains(P);  // cas limite, on relance la fonction.
                nbintersections += iseg;
            }
            return Useful.IsOdd(nbintersections);
        }

        private int intersectsegment(Vector2 A, Vector2 B, Vector2 I, Vector2 P)
        {
            Vector2 D = B - A;
            Vector2 E = P - I;
            float denom = D.X * E.Y - D.Y * E.X;
            if (denom == 0f)
                return -1;   // erreur, cas limite
            float t = -(A.X * E.Y - I.X * E.Y - E.X * A.Y + E.X * I.Y) / denom;
            if (t < 0 || t >= 1)
                return 0;
            float u = -(-D.X * A.Y + D.X * I.Y + D.Y * A.X - D.Y * I.X) / denom;
            if (u < 0 || u >= 1)
                return 0;
            return 1;
        }
        #endregion

        public override bool CollideLine(Line l) => CollideLine(l.A, l.B);
        public override bool CollideLine(in Vector2 A, in Vector2 B)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (CollideLines(A, B, vertices[i], vertices[(i + 1) % vertices.Count]))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CollideDroite(Droite d) => CollideDroite(d.A, d.B);
        public override bool CollideDroite(in Vector2 A, in Vector2 B)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (CollideLineDroite(vertices[i], vertices[(i + 1) % vertices.Count], A, B))
                {
                    return true;
                }
            }
            return false;
        }

        #region Move

        public override void Move(in float vx, in float vy)
        {
            center = new Vector2(center.X + vx, center.Y + vy);
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = new Vector2(vertices[i].X + vx, vertices[i].Y + vy);
            }
        }
        public override void Move(Vector2 shift)
        {
            center += shift;
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] += shift;
            }
        }
        public override void Move(Point shift)
        {
            center = new Vector2(center.X + shift.X, center.Y + shift.Y);
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = new Vector2(vertices[i].X + shift.X, vertices[i].Y + shift.Y);
            }
        }
        public override void MoveAt(in float posX, in float posY)
        {
            MoveAt(new Vector2(posX, posY));
        }
        public override void MoveAt(Vector2 shift)
        {
            Vector2 oldCenter = center;
            center = shift + defaultOffset;
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = center + (vertices[i] - oldCenter);
            }
        }
        public override void MoveAt(Point shift)
        {
            MoveAt(shift.ToVector2());
        }

        #endregion

        public override Hitbox ToHitbox()
        {
            float distMaxX = 0;
            float distMaxY = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector2 delta = center - vertices[i];
                distMaxX = Math.Max(delta.X, distMaxX);
                distMaxY = Math.Max(delta.Y, distMaxY);
            }
            Vector2 newOffset = new Vector2(-distMaxX, -distMaxY) + defaultOffset;
            return new Hitbox(sprite, new Rectangle((int)(center.X - distMaxX), (int)(center.Y - distMaxY), (int)(distMaxX * 2f), (int)(distMaxY * 2f)), newOffset);
        }
        public override Circle ToInclusiveCircle()
        {
            int maxDist = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                maxDist = (int)Math.Max(center.Distance(vertices[i]), maxDist);
            }
            return new Circle(sprite, center, defaultOffset, maxDist);
        }
        public override void SetScale(in Vector2 newScale, in Vector2 oldScale)
        {
            Vector2 ratio = newScale / oldScale;
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = center - (center - vertices[i]) * ratio;
            }
        }
        public override void Rotate(in float angle)
        {
            Vector2 O = center + defaultOffset;
            for (int i = 0; i < vertices.Count; i++)
            {
                float distOP = (vertices[i] - O).Length();
                float newAngle = Useful.Angle(O, vertices[i]) + angle;
                vertices[i] = new Vector2((float)(O.X + distOP * Math.Cos(newAngle)), (float)(O.Y + distOP * Math.Sin(newAngle)));
            }
            if(defaultOffset != Vector2.Zero)
            {
                float newAngleCenter = Useful.Angle(O, center) + angle;
                float distCenter = (center - O).Length();
                center = new Vector2((float)(O.X + distCenter * Math.Cos(newAngleCenter)), (float)(O.Y + distCenter * Math.Sin(newAngleCenter)));
                defaultOffset = O - center;
            }
            base.Rotate(angle);
        }
        public override void Draw(SpriteBatch spriteBacth)
        {
            if(Camera.mainCamera != null)
            {
                for (int i = 0; i < vertices.Count; i++)
                {
                    spriteBacth.DrawLine(Camera.mainCamera.ToScreenCoordinateSystem(vertices[i]), Camera.mainCamera.ToScreenCoordinateSystem(vertices[(i + 1) % vertices.Count]), color, thickness, 0f);
                }
                spriteBacth.DrawCircle(Camera.mainCamera.ToScreenCoordinateSystem(center), 3, color);
            }
        }
    }

    public class Hitbox : Collider
    {
        public Polygone rec;
        public float width, height;
        public override Color color { get => base.color; set { base.color = value; rec.color = value; } }

        public Hitbox(Sprite sprite, Rectangle hitbox) : base(sprite)
        {
            width = hitbox.Width;
            height = hitbox.Height;
            List<Vector2> vertices = new List<Vector2>
            {
                new Vector2(hitbox.X, hitbox.Y),
                new Vector2(hitbox.X + hitbox.Width, hitbox.Y),
                new Vector2(hitbox.X + hitbox.Width, hitbox.Y + hitbox.Height),
                new Vector2(hitbox.X, hitbox.Y + hitbox.Height)
            };
            rec = new Polygone(sprite, vertices, hitbox.Center.ToVector2());
            color = Color.Black;
            thickness = 2;
        }
        public Hitbox(Sprite sprite, Rectangle hitbox, Vector2 defaultOffset) : base(sprite, defaultOffset)
        {
            width = hitbox.Width;
            height = hitbox.Height;
            List<Vector2> vertices = new List<Vector2>
            {
                new Vector2(hitbox.X, hitbox.Y),
                new Vector2(hitbox.X + hitbox.Width, hitbox.Y),
                new Vector2(hitbox.X + hitbox.Width, hitbox.Y + hitbox.Height),
                new Vector2(hitbox.X, hitbox.Y + hitbox.Height)
            };
            rec = new Polygone(sprite, vertices, hitbox.Center.ToVector2());
            color = Color.Black;
            thickness = 2;
        }
        public Hitbox(Sprite sprite, Polygone rec, Vector2 defaultOffset) : base(sprite)
        {
            if(rec.vertices.Count == 4)
                this.rec = rec;
            width = rec.vertices[0].Distance(rec.vertices[1]);
            height = rec.vertices[0].Distance(rec.vertices[2]);
            this.defaultOffset = defaultOffset;
        }
        public override void Move(Point shift)
        {
            rec.Move(shift);
        }
        public override void Move(Vector2 shift)
        {
            rec.Move(shift);
        }
        public override void Move(in float vx, in float vy)
        {
            rec.Move(vx, vy);
        }
        public override void MoveAt(Point p)
        {
            rec.MoveAt(p);
        }
        public override void MoveAt(Vector2 p)
        {
            rec.MoveAt(p);
        }
        public override void MoveAt(in float vx, in float vy)
        {
            rec.MoveAt(vx, vy);
        }
        public override void Rotate(in float angle)
        {
            rec.Rotate(angle);
            base.Rotate(angle);
            rec.MoveAt(center);
        }
        public override bool Collide(Collider c)
        {
            if (c is Circle)
            {
                return CollideCirclePolygone((Circle)c, rec);
            }
            else if (c is Hitbox)
            {
                return CollideHitboxs(this, (Hitbox)c);
            }
            else if(c is Polygone)
            {
                return CollidePolygoneHitbox((Polygone)c, this);
            }
            else // capsule
            {
                return CollideHitboxCapsule(this, (Capsule)c);
            }
        }
        public override bool CollideDroite(Droite d) => CollideHitboxDroite(this, d);
        public override bool CollideDroite(in Vector2 A, in Vector2 B) => CollideHitboxDroite(this, A, B);
        public override bool CollideLine(Line l) => CollideHitboxLine(this, l);
        public override bool CollideLine(in Vector2 A, in Vector2 B) => CollideHitboxLine(this, A, B);

        public override Hitbox ToHitbox() => this;
        public override void SetScale(in Vector2 newScale, in Vector2 oldScale)
        {
            rec.SetScale(newScale, oldScale);
            width  *= newScale.X / oldScale.X;
            height *= newScale.Y / oldScale.Y;
        }

        public override Vector2 center
        {
            get => rec.center;
            protected set { Move(value - center); }
        }
        public override Point size
        {
            get => new Point((int)(rec.vertices[1].X - rec.vertices[0].X), (int)(rec.vertices[2].Y - rec.vertices[0].Y));
            protected set { }
        }
        public override bool Contains(Vector2 p) => rec.Contains(p);
        public override bool Contains(Point p) => rec.Contains(p);
        public override Collider Clone() => new Hitbox(this.sprite, rec, defaultOffset);
        public override Circle ToInclusiveCircle() => new Circle(sprite, rec.center, defaultOffset, MathF.Sqrt((width * width) + (height * height)) / 2f);
        public override string ToString() => rec.ToString();
        public override void Draw(SpriteBatch spriteBacth)
        {
            rec.Draw(spriteBacth);
        }
    }

    public class Circle : Collider
    {
        private static readonly Dictionary<string, List<Vector2>> circleCache = new Dictionary<string, List<Vector2>>();
        public int sidesNumber = 50;
        protected Vector2 _center;
        public override Vector2 center
        {
            get => _center;
            protected set
            {
                _center = value;
            }
        }
        public float radius;

        //Pour le Collider
        public Circle(Sprite sprite, in Vector2 center, in Vector2 defaultOffset, in float radius) : base(sprite, defaultOffset)
        {
            this.center = center;
            this.radius = radius;
            thickness = 1;
            color = Color.Black;
        }
        public Circle(Sprite sprite, Point center, Vector2 defaultOffset, int radius) : base(sprite, defaultOffset)
        {
            this.center = center.ToVector2();
            this.radius = radius;
            thickness = 1;
            color = Color.Black;
        }
        public Circle(Circle circle) : base(circle.sprite, circle.defaultOffset)
        {
            this.radius = circle.radius;
            this.center = circle.center;
            this.thickness = circle.thickness;
            this.sidesNumber = circle.sidesNumber;

        }
        public Circle() : base(null)
        {
            center = Vector2.Zero;
            radius = 1;
            color = Color.Black;
            thickness = 1;
        }
        public Circle(in Vector2 center) : base(null)
        {
            this.center = center;
            radius = 1;
            thickness = 1;
            color = Color.Black;
        }
        public Circle(in Vector2 center, in float radius) : base(null)
        {
            this.center = center;
            this.radius = radius;
            thickness = 1;
            color = Color.Black;
        }
        public Circle(in Vector2 center, in float radius, in float thickness) : base(null)
        {
            this.center = center;
            this.radius = radius;
            this.thickness = thickness;
            color = Color.Black;
        }
        public Circle(in Vector2 center, in float radius, in float thinkness, in int size) : base(null)
        {
            this.center = center;
            this.radius = radius;
            this.thickness = thinkness;
            this.sidesNumber = size;
            color = Color.Black;
        }

        #region CollideLine

        public override bool CollideLine(Line l) => CollideLine(l.A, l.B);
        public override bool CollideLine(in Vector2 A, in Vector2 B)
        {
            Vector2 u = new Vector2(B.X - A.X, B.Y - A.Y);
            Vector2 AC = new Vector2(center.X - A.X, center.Y - A.Y);
            float CI = Math.Abs(u.X * AC.Y - u.Y * AC.X) / u.Length();
            if (CI > radius)
                return false;
            else
            {
                Vector2 AB = new Vector2(B.X - A.X, B.Y - A.Y);
                AC = new Vector2(center.X - A.X, center.Y - A.Y);
                Vector2 BC = new Vector2(center.X - B.X, center.Y - B.Y);
                float pscal1 = AB.X * AC.X + AB.Y * AC.Y;  // produit scalaire
                float pscal2 = (-AB.X) * BC.X + (-AB.Y) * BC.Y;  // produit scalaire
                if (pscal1 >= 0 && pscal2 >= 0)
                    return true;   // I entre A et B, ok.
                // dernière possibilité, A ou B dans le cercle
                return Vector2.DistanceSquared(A, center) < radius * radius || Vector2.DistanceSquared(B, center) < radius * radius;
            }
        }
        public override bool CollideDroite(Droite d) => CollideDroite(d.A, d.B);
        public override bool CollideDroite(in Vector2 A, in Vector2 B)
        {
            Vector2 u = new Vector2(B.X - A.X, B.Y - A.Y);
            Vector2 AC = new Vector2(center.X - A.X, center.Y - A.Y);
            float numerateur = Math.Abs(u.X * AC.Y - u.Y * AC.X);// norme du vecteur v
            return numerateur / u.Length() < radius;
        }

        #endregion

        public override void Move(Vector2 shift)
        {
            center += shift;
        }
        public override void Move(Point shift)
        {
            Move(shift.ToVector2());
        }
        public override void Move(in float vx, in float vy)
        {
            center = new Vector2(center.X + vx, center.Y + vy);
        }
        public override void MoveAt(Vector2 p)
        {
            center = p + defaultOffset;
        }
        public override void MoveAt(Point p)
        {
            this.center = new Vector2(p.X + defaultOffset.X, p.Y + defaultOffset.Y);
        }
        public override void MoveAt(in float vx, in float vy)
        {
            center = new Vector2(vx + defaultOffset.X, vy + defaultOffset.Y);
        }

        public override bool Collide(Collider c)
        {
            if (c is Circle)
            {
                return CollideCircles(this, (Circle)c);
            }
            else if (c is Hitbox)
            {
                return CollideCircleHitbox(this, (Hitbox)c);
            }
            else if (c is Polygone)
            {
                return CollideCirclePolygone(this, (Polygone)c);
            }
            else //capsule
            {
                return CollideCircleCapsule(this, (Capsule)c);
            }
        }
        public override Hitbox ToHitbox() => new Hitbox(sprite, new Rectangle(new Point((int)(center.X - radius), (int)(center.Y - radius)), size), defaultOffset);
        public override Circle ToInclusiveCircle() => this;

        public override void SetScale(in Vector2 newScale, in Vector2 olsScale)
        {
            radius = radius * (((newScale.X + newScale.Y) / 2f) / ((olsScale.X + olsScale.Y) / 2f));
        }

        public override Point size
        {
            get => new Point((int)(2 * radius), ((int)(2 * radius)));
            protected set { }
        }
        public override bool Contains(Vector2 p) => center.SqrDistance(p) <= radius * radius;
        public override bool Contains(Point p) => center.SqrDistance(p) <= radius * radius;
        public override string ToString() => ("center : " + center.ToString() + " Radius : " + radius.ToString());
        public override Collider Clone()
        {
            Circle c = new Circle(center, radius, thickness, sidesNumber);
            c.defaultOffset = defaultOffset;
            return c;
        }

        public void Draw(SpriteBatch spriteBatch, in Color color)
        {
            Geometrie.DrawPoints(spriteBatch, center, CreateCircle(radius, sidesNumber), color, thickness);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Geometrie.DrawPoints(spriteBatch, Camera.mainCamera.ToScreenCoordinateSystem(center), CreateCircle(radius, sidesNumber), color, thickness);
        }

        private List<Vector2> CreateCircle(double radius, int sides)
        {
            // Look for a cached version of this circle
            string circleKey = radius + "x" + sides;
            if (circleCache.ContainsKey(circleKey))
            {
                return circleCache[circleKey];
            }

            List<Vector2> vectors = new List<Vector2>();

            const double max = 2d * Math.PI;
            double step = max / sides;

            for (double theta = 0.0; theta < max; theta += step)
            {
                vectors.Add(new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta))));
            }

            // then add the first vector again so it's a complete loop
            vectors.Add(new Vector2((float)(radius), 0f));

            // Cache this circle so that it can be quickly drawn next time
            circleCache.Add(circleKey, vectors);
            return vectors;
        }
        public static Circle operator *(Circle c, float f)
        {
            c.radius = (int)(c.radius * f);
            return c;
        }
        public static Circle operator *(Circle c, int f)
        {
            c.radius = c.radius * f;
            return c;
        }
    }

    public class Capsule : Collider
    {
        public enum CapsuleLayout
        {
            horizontally, vertically
        }

        public Circle c1, c2;
        public Hitbox hitbox;
        public override Color color { get => base.color; set { base.color = value; c1.color = value; c2.color = value; hitbox.color = value; } }

        public override Vector2 center
        {
            get => hitbox.center;
            protected set { Move(value - center); }
        }
        public override Point size
        {
            get => new Point((int)(c1.radius + c2.radius), (int)(c1.radius + c2.radius));
            protected set { }
        }
        public CapsuleLayout capsuleLayout;
        public Capsule(Sprite sprite, in Rectangle rec, CapsuleLayout capsuleLayout, in Vector2 defaultOffset) : base(sprite)
        {
            Builder(rec, capsuleLayout, defaultOffset);
        }
        public Capsule(Capsule c) : base(c.sprite, c.defaultOffset)
        {
            c1 = (Circle)c.c1.Clone();
            c2 = (Circle)c.c2.Clone();
            hitbox = (Hitbox)c.hitbox.Clone();
            color = c.color;
            thickness = c.thickness;
        }
        public Capsule(Sprite sprite, Rectangle rec, CapsuleLayout capsuleLayout, in Vector2 defaultOffset, Color color, int thickness = 1) : base(sprite)
        {
            this.color = color;
            c1.color = color;
            c2.color = color;
            hitbox.color = color;
            this.thickness = thickness;
            c1.thickness = thickness;
            c2.thickness = thickness;
            hitbox.thickness = thickness;
            Builder(rec, capsuleLayout, defaultOffset);
        }
        private void Builder(in Rectangle rec, CapsuleLayout capsuleLayout, in Vector2 defaultOffset)
        {
            this.defaultOffset = defaultOffset;
            hitbox = new Hitbox(sprite, rec, defaultOffset);
            this.capsuleLayout = capsuleLayout;
            if (capsuleLayout == CapsuleLayout.horizontally)
            {
                c1 = new Circle(sprite, new Vector2(rec.X, rec.Y + rec.Height / 2f), new Vector2(-rec.Width / 2f, 0f), rec.Height / 2);
                c2 = new Circle(sprite, new Vector2(rec.X + rec.Width, rec.Y + rec.Height / 2f), new Vector2(rec.Width / 2f, 0f), rec.Height / 2);
            }
            else // vertically
            {
                c1 = new Circle(sprite, new Vector2(rec.X + rec.Width / 2f, rec.Y), new Vector2(0, -rec.Height / 2f), rec.Width / 2);
                c2 = new Circle(sprite, new Vector2(rec.X + rec.Width / 2f, rec.Y + rec.Height), new Vector2(0, rec.Height / 2f), rec.Width / 2);
            }
            c1.MoveAt(center);
            c2.MoveAt(center);
        }
        public override Collider Clone() => new Capsule(this);

        public override bool CollideLine(Line l) => CollideLine(l.A, l.B);
        public override bool CollideLine(in Vector2 A, in Vector2 B)
        {
            return CollideHitboxLine(hitbox, A, B) || CollideCircleLine(c1, A, B) || CollideCircleLine(c2, A, B);
        }
        public override bool CollideDroite(Droite d) => CollideDroite(d.A, d.B);
        public override bool CollideDroite(in Vector2 A, in Vector2 B)
        {
            return CollideHitboxDroite(hitbox, A, B) || CollideCircleDroite(c1, A, B) || CollideCircleDroite(c2, A, B);
        }
        
        public override bool Collide(Collider c)
        {
            if (c is Circle)
            {
                return CollideCircleCapsule((Circle)c, this);
            }
            else if (c is Hitbox)
            {
                return CollideHitboxCapsule((Hitbox)c, this);
            }
            else if(c is Polygone)
            {
                return CollidePolygoneCapsule((Polygone)c, this);
            }
            else //Capsule
            {
                return CollideCapsules((Capsule)c, this);
            }
        }
        public override void Move(in float vx, in float vy)
        {
            c1.Move(vx, vy);
            c2.Move(vx, vy);
            hitbox.Move(vx, vy);
        }
        public override void Move(Point shift)
        {
            c1.Move(shift);
            c2.Move(shift);
            hitbox.Move(shift);
        }
        public override void Move(Vector2 shift)
        {
            c1.Move(shift);
            c2.Move(shift);
            hitbox.Move(shift);
        }
        public override void MoveAt(in float vx, in float vy)
        {
            hitbox.MoveAt(vx, vy);
            c1.MoveAt(hitbox.center);
            c2.MoveAt(hitbox.center);
        }
        public override void MoveAt(Point shift)
        {
            hitbox.MoveAt(shift);
            c1.MoveAt(hitbox.center);
            c2.MoveAt(hitbox.center);
        }
        public override void MoveAt(Vector2 shift)
        {
            hitbox.MoveAt(shift);
            c1.MoveAt(hitbox.center);
            c2.MoveAt(hitbox.center);
        }
        public override void Rotate(in float angle)
        {
            Vector2 offsetC1 = c1.center - center;
            Vector2 offsetC2 = c2.center - center;
            if (offsetC1 != Vector2.Zero)
            {
                float norme = offsetC1.Length();
                float angTotal = Useful.Angle(Vector2.Zero, offsetC1) + angle;
                offsetC1 = new Vector2((float)(norme * Math.Cos(angTotal)), (float)(norme * Math.Sin(angTotal)));
                c1.defaultOffset = offsetC1;
                c1.MoveAt(center);
            }
            if (offsetC2 != Vector2.Zero)
            {
                float norme = offsetC2.Length();
                float angTotal = Useful.Angle(Vector2.Zero, offsetC2) + angle;
                offsetC2 = new Vector2((float)(norme * Math.Cos(angTotal)), (float)(norme * Math.Sin(angTotal)));
                c2.defaultOffset = offsetC2;
                c2.MoveAt(center);
            }
            hitbox.Rotate(angle);
            base.Rotate(angle);
        }
        public override bool Contains(Point p) => hitbox.Contains(p) || c1.Contains(p) || c2.Contains(p);
        public override bool Contains(Vector2 p) => hitbox.Contains(p) || c1.Contains(p) || c2.Contains(p);
        public override Hitbox ToHitbox() => hitbox;
        public override Circle ToInclusiveCircle()
        {
            if(capsuleLayout == CapsuleLayout.horizontally)
            {
                float radius = Math.Max(hitbox.width + c1.radius + c2.radius, hitbox.height) / 2f;
                return new Circle(sprite, center, defaultOffset, radius);
            }
            else
            {
                float radius = Math.Max(hitbox.width, hitbox.height + c1.radius + c2.radius) / 2f;
                return new Circle(sprite, center, defaultOffset, radius);
            }
        }
        public override void SetScale(in Vector2 newScale, in Vector2 oldScale)
        {
            c1.SetScale(newScale, oldScale);
            c1.defaultOffset *= newScale / oldScale;
            c2.SetScale(newScale, oldScale);
            c2.defaultOffset *= newScale / oldScale;
            hitbox.SetScale(newScale, oldScale);
            c1.MoveAt(center);
            c2.MoveAt(center);
        }
        public override void Draw(SpriteBatch spriteBacth)
        {
            c1.Draw(spriteBacth);
            c2.Draw(spriteBacth);
            hitbox.Draw(spriteBacth);
        }
    }

    public class ColliderMaterial
    {
        private float _bounceness;
        public float bounceness
        {
            get => _bounceness;
            set
            {
                if (value >= 0)
                {
                    _bounceness = value;
                }
            }
        }
        public float friction;

        public ColliderMaterial(in float friction, in float bounceness)
        {
            this.friction = friction; this.bounceness = bounceness;
        }
        public ColliderMaterial Clone() => new ColliderMaterial(friction, bounceness);
    }
}
