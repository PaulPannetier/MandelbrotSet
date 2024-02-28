using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SME.Physic
{
    public static class Physics
    {
        //LayerMask : le masque souhaité pour la collision
        public static List<Sprite> OverlapCollider(Collider c)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                if (s.Collide(c, out Collider collider))
                {
                    result.Add(s);
                }
            }
            return result;
        }
        public static List<Sprite> OverlapCollider(Collider c, string layerMask)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                if (s.Collide(c, out Collider collider, layerMask))
                {
                    result.Add(s);
                }
            }
            return result;
        }
        public static List<Sprite> OverlapCollider(Collider c, List<string> layerMasks)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                if (s.Collide(c, out Collider collider, layerMasks))
                {
                    result.Add(s);
                }
            }
            return result;
        }
        public static List<Sprite> OverlapCircle(Circle c) => OverlapCollider(c);
        public static List<Sprite> OverlapCircle(Circle c, string layerMask) => OverlapCollider(c, layerMask);
        public static List<Sprite> OverlapCircle(Circle c, List<string> layerMasks) => OverlapCollider(c, layerMasks);
        public static List<Sprite> OverlapRectangle(Hitbox hitbox) => OverlapCollider(hitbox);
        public static List<Sprite> OverlapRectangle(Hitbox hitbox, string layerMask) => OverlapCollider(hitbox, layerMask);
        public static List<Sprite> OverlapRectangle(Hitbox hitbox, List<string> layerMasks) => OverlapCollider(hitbox, layerMasks);
        public static List<Sprite> OverlapPolygone(Polygone p) => OverlapCollider(p);
        public static List<Sprite> OverlapPolygone(Polygone p, string layerMask) => OverlapCollider(p, layerMask);
        public static List<Sprite> OverlapPolygone(Polygone p, List<string> layerMasks) => OverlapCollider(p, layerMasks);
        public static List<Sprite> OverlapCapsule(Capsule c) => OverlapCollider(c);
        public static List<Sprite> OverlapCapsule(Capsule c, string layerMask) => OverlapCollider(c, layerMask);
        public static List<Sprite> OverlapCapsule(Capsule c, List<string> layerMasks) => OverlapCollider(c, layerMasks);
        public static List<Sprite> RaycastHit(Line raycast)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                if(s.Collide(raycast, out Collider collider))
                {
                    result.Add(s);
                }
            }
            return result;
        }
        public static List<Sprite> RaycastHit(Line raycast, string layerMask)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                if (s.Collide(raycast, out Collider collider, layerMask))
                {
                    result.Add(s);
                }
            }
            return result;
        }
        public static List<Sprite> RaycastHit(Line raycast, List<string> layerMasks)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                if (s.Collide(raycast, out Collider collider, layerMasks))
                {
                    result.Add(s);
                }
            }
            return result;
        }
        public static List<Sprite> RaycastHit(Droite raycast)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                if (s.Collide(raycast, out Collider collider))
                {
                    result.Add(s);
                }
            }
            return result;
        }
        public static List<Sprite> RaycastHit(Droite raycast, string layerMask)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                if (s.Collide(raycast, out Collider collider, layerMask))
                {
                    result.Add(s);
                }
            }
            return result;
        }
        public static List<Sprite> RaycastHit(Droite raycast, List<string> layerMasks)
        {
            List<Sprite> result = new List<Sprite>();
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                if (s.Collide(raycast, out Collider collider, layerMasks))
                {
                    result.Add(s);
                }
            }
            return result;
        }
        public static bool Collide(in Vector2 point) => Collide(point, out Collider collider);
        public static bool Collide(in Vector2 point, out Collider collider)
        {
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                foreach (Collider c in s.lstColliders)
                {
                    if(c.Contains(point))
                    {
                        collider = c;
                        return true;
                    }
                }
            }
            collider = null;
            return false;
        }
        public static bool Collide(in Vector2 point, out Collider collider, string layerMask)
        {
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                foreach (Collider c in s.lstColliders)
                {
                    if (c.CompareMask(layerMask) && c.Contains(point))
                    {
                        collider = c;
                        return true;
                    }
                }
            }
            collider = null;
            return false;
        }
        public static bool Collide(in Vector2 point, out Collider collider, List<string> layerMasks)
        {
            foreach (Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                foreach (Collider c in s.lstColliders)
                {
                    if (c.CompareMask(layerMasks) && c.Contains(point))
                    {
                        collider = c;
                        return true;
                    }
                }
            }
            collider = null;
            return false;
        }
    }
}
