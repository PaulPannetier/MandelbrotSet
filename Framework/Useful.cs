using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.LinearAlgebra.Solvers;

//monogame
namespace SME
{
    public delegate string SerialyseFunction<T>(T obj);
    public delegate T DeserialyseFunction<T>(string s);

    public static class Random
    {
        private static System.Random random = new System.Random();
        public static void SetRandomSeed(in int seed)
        {
            random = new System.Random(seed);
            Noise2d.Reseed();
        }
        /// <summary>
        /// randomize de seed of the random, allow to have diffenrent random number at each lauch of the game
        /// </summary>
        public static void SetRandomSeed()
        {
            SetRandomSeed((int)DateTime.Now.Ticks);
        }
        /// <returns> A random integer between a and b, [|a, b|]</returns>
        public static int Rand(in int a, in int b) => random.Next(a, b + 1);
        /// <returns> A random float between 0 and 1, [0, 1]</returns>
        public static float Rand() => (float)random.Next(int.MaxValue) / (int.MaxValue - 1);
        /// <returns> A random float between a and b, [a, b]</returns>
        public static float Rand(in float a, in float b) => Rand() * Math.Abs(b - a) + a;
        /// <returns> A random int between a and b, [|a, b|[</returns>
        public static int RandExclude(in int a, in int b) => random.Next(a, b);
        /// <returns> A random double between a and b, [a, b[</returns>
        public static float RandExclude(in float a, in float b) => (float)random.NextDouble() * (Math.Abs(b - a)) + a;
        public static float RandExclude() => (float)random.NextDouble();
        public static float PerlinNoise(in float x, in float y) => Noise2d.Noise(x, y);
        public static Color RandomColor() => new Color(Rand(0, 255) / 255f, Rand(0, 255) / 255f, Rand(0, 255) / 255f, 1f);
        /// <returns> A random color with a random opacity</returns>
        public static Color RandomColorTransparent() => new Color(Rand(0, 255) / 255f, Rand(0, 255) / 255f, Rand(0, 255) / 255f, (float)random.NextDouble());
        /// <returns> A random Vector2 normalised</returns>
        public static Vector2 RandomVector2()
        {
            float angle = RandExclude(0f,  MathHelper.TwoPi);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        /// <returns> A random Vector2 with de magnitude in param</returns>
        public static Vector2 RandomVector2(in float magnitude)
        {
            float angle = RandExclude(0f, MathHelper.TwoPi);
            return new Vector2(magnitude * (float)Math.Cos(angle), magnitude * (float)Math.Sin(angle));
        }
        /// <returns> A random Vector2 with a randoml magnitude</returns>
        public static Vector2 RandomVector2(in float minMagnitude, in float maxMagnitude)
        {
            float angle = RandExclude(0f, MathHelper.TwoPi);
            float magnitude = Rand(minMagnitude, maxMagnitude);
            return new Vector2(magnitude * (float)Math.Cos(angle), magnitude * (float)Math.Sin(angle));
        }
        /// <returns> A random Vector3 normalised</returns>
        public static Vector3 RandomVector3()
        {
            float teta = Rand(0f, MathHelper.Pi);
            float phi = RandExclude(0f, MathHelper.TwoPi);
            return new Vector3((float)Math.Sin(teta) * (float)Math.Cos(phi), (float)Math.Sin(teta) * (float)Math.Sin(phi), (float)Math.Cos(teta));
        }
        /// <returns> A random Vector3 with de magnitude in param</returns>
        public static Vector3 RandomVector3(in float magnitude)
        {
            float teta = Rand(0f, MathHelper.Pi);
            float phi = RandExclude(0f, MathHelper.TwoPi);
            return new Vector3(magnitude * (float)Math.Sin(teta) * (float)Math.Cos(phi), magnitude * (float)Math.Sin(teta) * (float)Math.Sin(phi), magnitude * (float)Math.Cos(teta));
        }
        /// <returns> A random Vector3 with a random magnitude</returns>
        public static Vector3 RandomVector3(in float minMagnitude, in float maxMagnitude)
        {
            float teta = Rand(0f, MathHelper.Pi);
            float phi = RandExclude(0f, MathHelper.TwoPi);
            float magnitude = Rand(minMagnitude, maxMagnitude);
            return new Vector3(magnitude * (float)Math.Sin(teta) * (float)Math.Cos(phi), magnitude * (float)Math.Sin(teta) * (float)Math.Sin(phi), magnitude * (float)Math.Cos(teta));
        }
        private static class Noise2d
        {
            private static int[] _permutation;

            private static Vector2[] _gradients;

            static Noise2d()
            {
                CalculatePermutation(out _permutation);
                CalculateGradients(out _gradients);
            }

            private static void CalculatePermutation(out int[] p)
            {
                p = Enumerable.Range(0, 256).ToArray();

                /// shuffle the array
                for (var i = 0; i < p.Length; i++)
                {
                    var source = RandExclude(0, p.Length);

                    var t = p[i];
                    p[i] = p[source];
                    p[source] = t;
                }
            }

            /// <summary>
            /// generate a new permutation.
            /// </summary>
            public static void Reseed()
            {
                CalculatePermutation(out _permutation);
            }

            private static void CalculateGradients(out Vector2[] grad)
            {
                grad = new Vector2[256];

                for (var i = 0; i < grad.Length; i++)
                {
                    Vector2 gradient;
                    do
                    {
                        gradient = new Vector2((RandExclude() * 2f - 1f), (RandExclude() * 2f - 1f));
                    }
                    while (gradient.LengthSquared() >= 1);

                    gradient.Normalize();

                    grad[i] = gradient;
                }
            }

            private static float Drop(float t)
            {
                t = Math.Abs(t);
                return 1f - t * t * t * (t * (t * 6 - 15) + 10);
            }

            private static float Q(in float u, in float v)
            {
                return Drop(u) * Drop(v);
            }

            public static float Noise(in float x, in float y)
            {
                var cell = new Vector2((float)Math.Floor(x), (float)Math.Floor(y));

                var total = 0f;

                var corners = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };

                foreach (var n in corners)
                {
                    var ij = cell + n;
                    var uv = new Vector2(x - ij.X, y - ij.Y);

                    var index = _permutation[(int)ij.X % _permutation.Length];
                    index = _permutation[(index + (int)ij.Y) % _permutation.Length];

                    var grad = _gradients[index % _gradients.Length];

                    total += Q(uv.X, uv.Y) * Vector2.Dot(grad, uv);
                }
                return Math.Max(Math.Min(total, 1f), -1f);
            }
        }

        //Generer les lois de probas
        public static int Bernoulli(in float p) => Rand() <= p ? 1 : 0;
        public static int Binomial(in int n, in int p)
        {
            int count = 0;
            for (int i = 0; i < n; i++)
                count += Bernoulli(p);
            return count;
        }
        public static float Expodential(in float lambda) => (-1f / lambda) * MathF.Log(Rand());
        public static int Poisson(in float lambda)
        {
            float x = Rand();
            int n = 0;
            while (x > MathF.Exp(-lambda))
            {
                x *= Rand();
                n++;
            }
            return n;
        }
        public static int Geometric(in float p)
        {
            int count = 0;
            do
            {
                count++;
            } while (Bernoulli(p) == 0);
            return count;
        }
        private static float N01() => MathF.Sqrt(-2f * MathF.Log(Rand())) * MathF.Cos(MathHelper.TwoPi * Rand());
        public static float Normal(in float esp, in float sigma) => N01() * sigma + esp;
    }

    public static class Useful
    {
        #region Colordeeper/lighter
        /// <summary>
        /// Return a Color deeper than the color in argument
        /// </summary>
        /// <param name="c">The color to change</param>
        /// <param name="percent">le coeff €[0, 1] d'assombrissement</param>
        /// <returns></returns>
        public static Color ColorDeeper(this Color c, float coeff) => new Color((byte)(c.R * (1f - coeff)), (byte)(c.G * (1f - coeff)), (byte)(c.B * (1f - coeff)), c.A);
        /// <summary>
        /// Return a Color lighter than the color in argument
        /// </summary>
        /// <param name="c">The color to change</param>
        /// <param name="percent">le coeff €[0, 1] de luminosité</param>
        /// <returns></returns>
        public static Color ColorLighter(this Color c, float coeff) => new Color((byte)((255 - c.R) * coeff) + c.R, (byte)((255 - c.G) * coeff) + c.G, (byte)((255 - c.B) * coeff) + c.B, c.A);
        #endregion

        #region Vector 

        //Vector2
        public static float SqrDistance(this Vector2 v, in Vector2 a) => (a.X - v.X) * (a.X - v.X) + (a.Y - v.Y) * (a.Y - v.Y);
        public static float SqrDistance(this Vector2 v, in Point a) => (a.X - v.X) * (a.X - v.X) + (a.Y - v.Y) * (a.Y - v.Y);
        public static float Distance(this Vector2 v, in Vector2 a) => (float)Math.Sqrt(v.SqrDistance(a));
        public static float Distance(this Vector2 v, in Point a) => (float)Math.Sqrt(v.SqrDistance(a));
        /// <summary>
        /// Le produit scalaire
        /// </summary>
        public static float DotProduct(this Vector2 v, in Vector2 a) => v.X * a.X + v.Y * a.Y;
        /// <summary>
        /// Le produit vectorielle
        /// </summary>
        public static Vector3 CrossProduct(this Vector2 a, in Vector2 b) => new Vector3(0f, 0f, a.X * b.Y - a.Y * b.X);
        public static bool IsCollinear(this Vector2 a, in Vector2 b) => Math.Abs((b.X / a.X) - (b.Y / a.Y)) < 0.007f * Math.Abs(b.Y / a.Y);

        //Vector3
        public static float SqrDistance(this Vector3 v, in Vector3 a) => (a.X - v.X) * (a.X - v.X) + (a.Y - v.Y) * (a.Y - v.Y) + (a.Z - v.Z) * (a.Z - v.Z);
        public static float Distance(this Vector3 v, in Vector3 a) => (float)Math.Sqrt(v.SqrDistance(a));
        /// <summary>
        /// Le produit scalaire
        /// </summary>
        public static float DotProduct(this Vector3 v, in Vector3 a) => v.X * a.X + v.Y * a.Y + v.Z * a.Z;
        /// <summary>
        /// Le produit vectorielle
        /// </summary>
        public static Vector3 CrossProduct(this Vector3 a, in Vector3 b) => new Vector3(a.Y * b.Z - a.Z * b.Y, -a.X * b.Z + a.Z * b.X, a.X * b.Y - a.Y * b.X);
        public static bool IsCollinear(this Vector3 a, in Vector3 b) => Math.Abs(b.X / a.X - b.Y / a.Y) < 0.007f * Math.Abs(b.Y / a.Y) &&
                                                                        Math.Abs(b.X / a.X - b.Z / a.Z) < 0.007f * Math.Abs(b.Z / a.Z) &&
                                                                        Math.Abs(b.Y / a.Y - b.Z / a.Z) < 0.007f * Math.Abs(b.Z / a.Z);
           
        /// <summary>
        /// Renvoie un vecteur orthogonal à ce vecteur
        /// </summary>
        public static Vector2 NormalVector(this Vector2 v)
        {
            if(Math.Abs(v.Y) > 0.001f)
            {
                return new Vector2(10f, (-v.X * 10f) / v.Y);
            }
            if (Math.Abs(v.X) > 0.001f)
            {
                return new Vector2((-v.Y * 10f) / v.X, 10f);
            }
            return Vector2.Zero;
        }

        #endregion

        #region Angle

        public static float ToRad(float angle) => (float)(((angle * Math.PI) / 180f) % (2 * Math.PI));
        public static float ToDegrees(float angle) => (float)(((angle * 180f) / Math.PI) % 360);

        /// <returns> l'angle entre les vecteurs (1, 0) et (pos2 - pos1) compris entre 0 et 2pi radian</returns>
        public static float Angle(in Vector2 pos1, in Vector2 pos2) => MathF.Atan2(pos1.Y - pos2.Y, pos1.X - pos2.X) + MathF.PI;
        /// <summary>
        /// Renvoie l'angle minimal (pos1, center, pos2)
        /// </summary>
        public static float Angle(in Vector2 center, in Vector2 pos1, in Vector2 pos2)
        {
            float ang1 = Angle(center, pos1);
            float ang2 = Angle(center, pos2);
            float diff = Math.Abs(ang1 - ang2);
            return Math.Min(diff, MathHelper.TwoPi - diff);
        }
        /// <summary>
        /// Renvoie si pour aller de l'angle 1 vers l'angle 2 le plus rapidement il faut tourner à droite au à gauche
        /// </summary>
        public static void DirectionAngle(float ang1, float ang2, out bool right)
        {
            WrapAngle(ang1);
            WrapAngle(ang2);
            float diff = Math.Abs(ang1 - ang2);
            float angMin = Math.Min(diff, 2f * MathHelper.Pi - diff);
            right = Math.Abs((ang1 + angMin) % (MathHelper.TwoPi) - ang2) < 0.1f;
        }
        /// <summary>
        /// Renvoie l'angle en radian égal à l'angle en param mais dans [0, 2π[
        /// </summary>
        public static float WrapAngle(in float angle) => Clamp(0f, MathHelper.TwoPi, angle);
        /// <returns> value if value is in the range [a, b], a or b otherwise</returns>
        public static float MarkOut(in float min, in float max, in float value) => Math.Max(Math.Min(value, max), min);

        /// <returns> a like a = value % (end -  start) + start, a€[start, end[ /returns>
        public static float Clamp(in float start, in float end, in float value)
        {
            if (end < start)
                return Clamp(end, start, value);
            if (end == start)
                return start;

            if (value < end && value >= start)
                return value;
            else
            {
                float modulo = end - start;
                float result = (value % modulo) + start;
                if (result >= end)
                    return result - modulo;
                if (result < start)
                    return result + modulo;
                return result;
            }
        }

        #endregion

        #region Lerp
        /// <returns>The linear interpolation between start and end, t€[0f, 1f]</returns>
        public static float Lerp(in float start, in float end, in float t) => (end - start) * t + start;
        /// <returns>The linear interpolation between start and end, t€[0f, 1f]</returns>
        public static Color Lerp(in Color start, in Color end, in float t)
        {
            float R = Lerp(start.R, end.R, t) / 255f;
            float G = Lerp(start.G, end.G, t) / 255f;
            float B = Lerp(start.B, end.B, t) / 255f;
            float A = Lerp(start.A, end.A, t) / 255f;
            return new Color(R, G, B, A);
        }

        #endregion

        public static decimal Sqrt(in decimal x)
        {
            if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");

            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do
            {
                previous = current;
                if (previous == 0.0M) return 0;
                current = (previous + x / previous) * 0.5m;
            }
            while (Math.Abs(previous - current) > 0.0M);
            return current;
        }

        public static decimal Abs(in decimal x) => x >= 0m ? x : -x;

        /// <summary>
        /// Impair
        /// </summary>
        public static bool IsOdd(in int number) => number % 2 == 1;
        /// <summary>
        /// Pair
        /// </summary>
        public static bool IsEven(int number) => number % 2 == 0;
        
        /// <summary>
        /// Renvoie la valeur arrondi de n
        /// </summary>
        public static int Round(in float n) => (n - MathF.Floor(n)) >= 0.5f ? (int)n + 1 : (int)n;
        /// <summary>
        /// Renvoie la valeur arrondi de n au nombre de décimales en param, ex : Round(51.6854, 2) => 51.69
        /// </summary>
        public static float Round(in float n, in int nbDecimals)
        {
            float npow = n * MathF.Pow(10, nbDecimals);
            return npow - (int)(npow) >= 0.5f ? (((int)(npow + 1)) / MathF.Pow(10, nbDecimals)) : (((int)npow) / MathF.Pow(10, nbDecimals));
        }

        #region CopieArray
        public static T[,] Clone<T>(this T[,] Array)
        {
            T[,] a = new T[Array.GetLength(0), Array.GetLength(1)];
            for (int l = 0; l < a.GetLength(0); l++)
            {
                for (int c = 0; c < a.GetLength(1); c++)
                {
                    a[l, c] = Array[l, c];
                }
            }
            return a;
        }
        #endregion

        #region GetSubArray
        /// <summary>
        /// Retourne le sous tableau de Array, cad Array[IndexStart]
        /// </summary>
        /// <param name="indexStart">l'index de la première dimension de Array</param>
        public static T[,,] GetSubArray<T>(this T[,,,] Array, in int indexStart = 0)
        {
            T[,,] a = new T[Array.GetLength(1), Array.GetLength(2), Array.GetLength(3)];
            for (int l = 0; l < a.GetLength(0); l++)
            {
                for (int c = 0; c < a.GetLength(1); c++)
                {
                    for (int i = 0; i < a.GetLength(2); i++)
                    {
                        a[l, c, i] = Array[indexStart, l, c, i];
                    }
                }
            }
            return a;
        }
        #endregion

        public static bool CaseExistArray<T>(in T[,] tab, int l, int c) => l >= 0 && c >= 0 && l < tab.GetLength(0) && c < tab.GetLength(1);

        #region Affiche Array    

        public static void Show<T>(this T[] tab)
        {
            string text = "[ ";
            for (int l = 0; l < tab.GetLength(0); l++)
            {
                text += tab[l].ToString() + ", ";
            }
            text = text.Remove(text.Length - 2, 2);
            text += " ]";
            Debug.WriteLine(text);
        }
        public static void ShowArray<T>(this T[,] tab)
        {
            string text = "";
            for (int l = 0; l < tab.GetLength(0); l++)
            {
                text = "[ ";
                for (int c = 0; c < tab.GetLength(1); c++)
                {
                    text += tab[l, c].ToString() + ", ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ],";
                Debug.WriteLine(text);
            }
        }
        public static void ShowArray<T>(this T[,,] tab)
        {
            string text = "";
            for (int l = 0; l < tab.GetLength(0); l++)
            {
                text += "[ ";
                for (int c = 0; c < tab.GetLength(1); c++)
                {
                    text += "[ ";
                    for (int i = 0; i < tab.GetLength(2); i++)
                    {
                        text += tab[l, c, i].ToString() + ", ";
                    }
                    text = text.Remove(text.Length - 2, 2);
                    text += " ], ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 3, 3);
            text += "]";
            Debug.WriteLine(text);
        }
        public static void ShowArray<T>(this T[,,,] tab)
        {
            string text = "";
            for (int l = 0; l < tab.GetLength(0); l++)
            {
                text += "[ ";
                for (int c = 0; c < tab.GetLength(1); c++)
                {
                    text += "[ ";
                    for (int i = 0; i < tab.GetLength(2); i++)
                    {
                        text += "[ ";
                        for (int j = 0; j < tab.GetLength(3); j++)
                        {
                            text += tab[l, c, i, j].ToString() + ", ";
                        }
                        text = text.Remove(text.Length - 2, 2);
                        text += " ], ";
                    }
                    text = text.Remove(text.Length - 2, 2);
                    text += " ], ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 3, 3);
            text += "]";
            Debug.WriteLine(text);
        }
        public static void ShowArray<T>(this T[,,,,] tab)
        {
            string text = "";
            for (int l = 0; l < tab.GetLength(0); l++)
            {
                text += "[ ";
                for (int c = 0; c < tab.GetLength(1); c++)
                {
                    text += "[ ";
                    for (int i = 0; i < tab.GetLength(2); i++)
                    {
                        text += "[ ";
                        for (int j = 0; j < tab.GetLength(3); j++)
                        {
                            text += "[ ";
                            for (int k = 0; k < tab.GetLength(4); k++)
                            {
                                text += tab[l, c, i, j, k].ToString() + ", ";
                            }
                            text = text.Remove(text.Length - 2, 2);
                            text += " ], ";
                        }
                        text = text.Remove(text.Length - 2, 2);
                        text += " ], ";
                    }
                    text = text.Remove(text.Length - 2, 2);
                    text += " ], ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 3, 3);
            text += "]";
            Debug.WriteLine(text);
        }
        #endregion

        #region Normalise Array

        /// <summary>
        /// Normalise tout les éléments de l'array pour obtenir des valeur entre 0f et 1f, ainse le min de array sera 0f, et le max 1f.
        /// </summary>
        /// <param name="array">The array to normalised</param>
        public static void Normalise(this float[] array)
        {
            float min = float.MaxValue, max = float.MinValue;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                    min = array[i];
                if (array[i] > max)
                    max = array[i];
            }
            float maxMinusMin = max - min;
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (array[i] - min) / maxMinusMin;
            }
        }
        /// <summary>
        /// Change array like the sum of each element make 1f
        /// </summary>
        /// <param name="array">the array to change, each element must to be positive</param>
        public static void GetProbabilityArray(this float[] array)
        {
            float sum = 0f;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < 0f)
                {
                    Debug.WriteLine("Array[" + i + "] must to be positive : " + array[i]);
                    return;
                }
                sum += array[i];
            }
            for (int i = 0; i < array.Length; i++)
            {
                array[i] /= sum;
            }
        }
        #endregion

        #region Shuffle

        public static void Shuffle<T>(this List<T> list)
        {
            for(int i = list.Count; i >= 0; i--)
            {
                int j = Random.Rand(0, i);
                T tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
        /// <summary>
        /// Shuffle a little bit the list, reproduce approximately the real life
        /// </summary>
        /// <param name="percentage">The percentage to shuffle between 0 and 1</param>
        public static void ShufflePartialy<T>(this List<T> list, in float percentage)
        {
            int nbPermut = (int)(list.Count * percentage);
            for (int i = 0; i < nbPermut; i++)
            {
                int randIndex1 = Random.RandExclude(0, list.Count);
                int randIndex2 = Random.RandExclude(0, list.Count);
                T temp = list[randIndex1];
                list[randIndex1] = list[randIndex2];
                list[randIndex2] = temp;
            }
        }

        #endregion

        public static List<T> Clone<T>(this List<T> lst) => new List<T>(lst);

        private static string[] letter = new string[36] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        private static string Troncate(string mot)
        {
            string newMot = mot;
            for (int i = 0; i < mot.Length; i++)
            {
                string s = mot.Substring(i, 1);
                if (s == "," || s == ".")
                {
                    newMot = newMot.Remove(i, mot.Length - i);
                    break;
                }
            }
            return newMot;
        }

        public static int ConvertStringToInt(string number)
        {
            int nb = 0;
            number = Troncate(number);
            for (int i = number.Length - 1; i >= 0; i--)
            {
                string carac = number.Substring(i, 1);
                for (int j = 26; j <= 35; j++)
                {
                    if (carac == letter[j])
                    {
                        int n = j - 26;
                        nb += n * (int)Math.Pow(10, number.Length - 1 - i);
                        break;
                    }
                }
            }
            if (number.Substring(0, 1) == "-")
                nb *= -1;

            return nb;
        }

        public static float ConvertStringToFloat(string number)
        {
            float result = 0;
            string partieEntiere = number;
            string partieDecimal = "";

            int indexComa = 0;
            for (int i = 0; i < number.Length; i++)
            {
                string s = number.Substring(i, 1);
                if (s == "," || s == ".")
                {
                    partieDecimal = partieEntiere.Substring(i + 1, partieEntiere.Length - (i + 1));
                    partieEntiere = partieEntiere.Remove(i, partieEntiere.Length - i);
                    indexComa = i;
                    break;
                }
            }
            //part entière
            result = ConvertStringToInt(partieEntiere);
            //part decimal
            for (int i = 0; i < partieDecimal.Length; i++)
            {
                string carac = partieDecimal.Substring(i, 1);
                for (int j = 26; j <= 35; j++)
                {
                    if (carac == letter[j])
                    {
                        int n = j - 26; //n € {0,1,2,3,4,5,6,7,8,9}
                        result += n * MathF.Pow(10, -(i + 1));
                        break;
                    }
                }
            }
            return result;
        }

        #region SumList
        /// <summary>
        /// Retourne lst1 union lst2
        /// </summary>
        /// <param name="lst1">La première liste</param>
        /// <param name="lst2">La seconde liste</param>
        /// <param name="doublon">SI on autorise ou pas les doublons</param>
        /// <returns></returns>        
        public static List<T> SumList<T>(in List<T> lst1, in List<T> lst2, bool doublon = false)//pas de doublon par defaut
        {
            List<T> result = new List<T>();
            foreach (T nb in lst1)
            {
                if (doublon || !result.Contains(nb))
                    result.Add(nb);
            }
            foreach (T nb in lst2)
            {
                if (doublon || !result.Contains(nb))
                    result.Add(nb);
            }
            return result;
        }
        public static List<T> SumList<T>(this List<T> lst1, in List<T> lst2, bool doublon = false)//pas de doublon par defaut
        {
            return SumList(lst1, lst2);
        }
        public static void Add<T>(this List<T> lst1, in List<T> lstToAdd, bool doublon = false)//pas de doublon par defaut
        {
            if (doublon)
            {
                foreach (T element in lstToAdd)
                {
                    lst1.Add(element);
                }
            }
            else
            {
                foreach (T element in lstToAdd)
                {
                    if (lst1.Contains(element))
                    {
                        continue;
                    }
                    lst1.Add(element);
                }
            }

        }
        #endregion

        #region ConvertStingToArray
        public static object ConvertStingToArray(string tab, out int dimension)
        {
            tab = tab.Replace(" ", "");//on enlève tout les espaces
            int dim = 0;//on calcule la dim
            int compteurDim = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() == "[")
                    compteurDim++;
                if (tab[i].ToString() == "]")
                    compteurDim--;
                dim = Math.Max(dim, compteurDim);
            }
            dimension = dim;
            switch (dim)
            {
                case 1:
                    return ConvertStringToArray1(tab);
                case 2:
                    return ConvertStringToArray2(tab);
                case 3:
                    return ConvertStringToArray3(tab);
                case 4:
                    return ConvertStringToArray4(tab);
                case 5:
                    return ConvertStringToArray5(tab);
                default:
                    throw new Exception("To many dim in " + tab + " max dim is 5");
            }
        }
        private static object ConvertStringToArray1(string tab)
        {
            List<string> value = new List<string>();
            string val = "";
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() == "," || tab[i].ToString() == "]" || tab[i].ToString() == "[")
                {
                    if (val != "")
                        value.Add(val);
                    val = "";
                }
                else
                {
                    val += tab[i].ToString();
                }
            }

            int[] result = new int[value.Count];
            for (int i = 0; i < value.Count; i++)
            {
                result[i] = ConvertStringToInt(value[i]);
            }
            return result;
        }
        private static object ConvertStringToArray2(string tab)
        {
            //"[[1,2,3,4],[4,5,6]]" va retourné [[1, 2, 3, 4], [4, 5, 6, -1]]
            int nbline = -1, nbCol = 0;
            int compteurDim = -1;
            int compteurCol = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() == "[")
                    compteurDim++;
                if (tab[i].ToString() == "]")
                {
                    compteurDim--;
                    nbline++;
                }
                if (compteurDim == 1)
                {
                    if (tab[i].ToString() == ",")
                    {
                        compteurCol++;
                        nbCol = Math.Max(nbCol, compteurCol + 1);
                    }
                }
                else
                {
                    compteurCol = 0;
                }
            }
            int[,] result = new int[nbline, nbCol];//on crée et initialise le tab;
            for (int l = 0; l < result.GetLength(0); l++)
            {
                for (int c = 0; c < result.GetLength(1); c++)
                {
                    result[l, c] = -1;
                }
            }
            //on remplit le resulat
            compteurDim = -1;
            compteurCol = 0;
            int compteurLine = -2;
            string value = "";
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() != "[" && tab[i].ToString() != "]" && tab[i].ToString() != ",")
                {
                    value += tab[i].ToString();
                }
                if (tab[i].ToString() == "[")
                {
                    compteurDim++;
                    compteurLine++;
                    compteurCol = 0;
                }
                else
                {
                    if (tab[i].ToString() == "]")
                    {
                        compteurDim--;
                        if (value != "")
                        {
                            result[compteurLine, compteurCol] = ConvertStringToInt(value);
                            value = "";
                        }
                    }
                    else
                    {
                        if (tab[i].ToString() == ",")
                        {
                            if (value != "")
                            {
                                result[compteurLine, compteurCol] = ConvertStringToInt(value);
                                value = "";
                                compteurCol++;
                            }
                        }
                    }
                }
            }

            return result;
        }
        private static object ConvertStringToArray3(string tab)
        {
            return null;
        }
        private static object ConvertStringToArray4(string tab)
        {
            int nbDim0 = 1, nbDim1 = 1, nbDim2 = 1, nbDim3 = 1;
            int compteurDim0 = 0, compteurDim1 = 0, compteurDim2 = 0, compteurDim3 = 0;
            int compteurDim = -1;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() == "[")
                    compteurDim++;
                if (tab[i].ToString() == "]")
                    compteurDim--;
                switch (compteurDim)
                {
                    case 0:
                        compteurDim1 = compteurDim2 = compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim0++;
                            nbDim0 = Math.Max(nbDim0, compteurDim0 + 1);
                        }
                        break;
                    case 1:
                        compteurDim2 = compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim1++;
                            nbDim1 = Math.Max(nbDim1, compteurDim1 + 1);
                        }
                        break;
                    case 2:
                        compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim2++;
                            nbDim2 = Math.Max(nbDim2, compteurDim2 + 1);
                        }
                        break;
                    case 3:
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim3++;
                            nbDim3 = Math.Max(nbDim3, compteurDim3 + 1);
                        }
                        break;
                    default:
                        //throw new Exception("blabla");
                        break;
                }
            }

            int[,,,] result = new int[nbDim0, nbDim1, nbDim2, nbDim3];//on crée et initialise le tab;
            for (int a = 0; a < result.GetLength(0); a++)
            {
                for (int b = 0; b < result.GetLength(1); b++)
                {
                    for (int c = 0; c < result.GetLength(2); c++)
                    {
                        for (int d = 0; d < result.GetLength(3); d++)
                        {
                            result[a, b, c, d] = -1;
                        }
                    }
                }
            }
            compteurDim0 = compteurDim1 = compteurDim2 = compteurDim3 = 0;
            compteurDim = -1;
            string value = "";
            string text;

            for (int i = 0; i < tab.Length; i++)
            {
                text = tab[i].ToString();
                if (tab[i].ToString() != "[" && tab[i].ToString() != "]" && tab[i].ToString() != ",")
                    value += tab[i].ToString();
                if (tab[i].ToString() == "[")
                {
                    compteurDim++;
                    //compteurDim0++;
                }
                if (tab[i].ToString() == "]")
                {
                    compteurDim--;
                    if (value != "")
                    {
                        result[compteurDim0, compteurDim1, compteurDim2, compteurDim3] = ConvertStringToInt(value);
                        value = "";
                    }
                }
                switch (compteurDim)
                {
                    case 0:
                        compteurDim1 = compteurDim2 = compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim0++;
                        }
                        break;
                    case 1:
                        compteurDim2 = compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim1++;
                        }
                        break;
                    case 2:
                        compteurDim3 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim2++;
                        }
                        break;
                    case 3:
                        if (tab[i].ToString() == ",")
                        {
                            result[compteurDim0, compteurDim1, compteurDim2, compteurDim3] = ConvertStringToInt(value);
                            value = "";
                            compteurDim3++;
                        }
                        break;
                    default:
                        //throw new Exception("blabla");
                        break;
                }
            }
            return result;
        }
        private static object ConvertStringToArray5(string tab)
        {
            int nbDim0 = 1, nbDim1 = 1, nbDim2 = 1, nbDim3 = 1, nbDim4 = 1;
            int compteurDim0 = 0, compteurDim1 = 0, compteurDim2 = 0, compteurDim3 = 0, compteurDim4 = 0;
            int compteurDim = -1;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() == "[")
                    compteurDim++;
                if (tab[i].ToString() == "]")
                    compteurDim--;
                switch (compteurDim)
                {
                    case 0:
                        compteurDim1 = compteurDim2 = compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim0++;
                            nbDim0 = Math.Max(nbDim0, compteurDim0 + 1);
                        }
                        break;
                    case 1:
                        compteurDim2 = compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim1++;
                            nbDim1 = Math.Max(nbDim1, compteurDim1 + 1);
                        }
                        break;
                    case 2:
                        compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim2++;
                            nbDim2 = Math.Max(nbDim2, compteurDim2 + 1);
                        }
                        break;
                    case 3:
                        compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim3++;
                            nbDim3 = Math.Max(nbDim3, compteurDim3 + 1);
                        }
                        break;
                    case 4:
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim4++;
                            nbDim4 = Math.Max(nbDim4, compteurDim4 + 1);
                        }
                        break;
                    default:
                        //throw new Exception("blabla");
                        break;
                }
            }

            int[,,,,] result = new int[nbDim0, nbDim1, nbDim2, nbDim3, nbDim4];//on crée et initialise le tab;
            for (int a = 0; a < result.GetLength(0); a++)
            {
                for (int b = 0; b < result.GetLength(1); b++)
                {
                    for (int c = 0; c < result.GetLength(2); c++)
                    {
                        for (int d = 0; d < result.GetLength(3); d++)
                        {
                            for (int e = 0; e < result.GetLength(4); e++)
                            {
                                result[a, b, c, d, e] = -1;
                            }
                        }
                    }
                }
            }
            compteurDim0 = compteurDim1 = compteurDim2 = compteurDim3 = compteurDim4 = 0;
            compteurDim = -1;
            string value = "";

            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i].ToString() != "[" && tab[i].ToString() != "]" && tab[i].ToString() != ",")
                    value += tab[i].ToString();
                if (tab[i].ToString() == "[")
                {
                    compteurDim++;
                    //compteurDim0++;
                }
                if (tab[i].ToString() == "]")
                {
                    compteurDim--;
                    if (value != "")
                    {
                        result[compteurDim0, compteurDim1, compteurDim2, compteurDim3, compteurDim4] = ConvertStringToInt(value);
                        value = "";
                    }
                }
                switch (compteurDim)
                {
                    case 0:
                        compteurDim1 = compteurDim2 = compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim0++;
                        }
                        break;
                    case 1:
                        compteurDim2 = compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim1++;
                        }
                        break;
                    case 2:
                        compteurDim3 = compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim2++;
                        }
                        break;
                    case 3:
                        compteurDim4 = 0;
                        if (tab[i].ToString() == ",")
                        {
                            compteurDim3++;
                        }
                        break;
                    case 4:
                        if (tab[i].ToString() == ",")
                        {
                            result[compteurDim0, compteurDim1, compteurDim2, compteurDim3, compteurDim4] = ConvertStringToInt(value);
                            value = "";
                            compteurDim4++;
                        }
                        break;
                    default:
                        //throw new Exception("blabla");
                        break;
                }
            }
            return result;
        }
        #endregion

        #region ConvertArrayToString
        public static string ConvertArrayToString<T>(in object array, in int dimension)
        {
            return ConvertArrayToString<T>(array, dimension, ToString);
        }
        private static string ToString<T>(T obj) => obj.ToString();

        public static string ConvertArrayToString<T>(in object array, in int dimension, SerialyseFunction<T> convertFunction)
        {
            switch (dimension)
            {
                case 1:
                    return ConvertArrayToString1((T[])array, convertFunction);
                case 2:
                    return ConvertArrayToString2((T[,])array, convertFunction);
                case 3:
                    return ConvertArrayToString3((T[,,])array, convertFunction);
                case 4:
                    return ConvertArrayToString4((T[,,,])array, convertFunction);
                case 5:
                    return ConvertArrayToString5((T[,,,,])array, convertFunction);
                default:
                    throw new Exception("Too many dimension in ConvertArrayToString, maximun 5");
            }
        }

        private static string ConvertArrayToString1<T>(in T[] array, SerialyseFunction<T> convertFunction)
        {
            string result = "[";
            for (int i = 0; i < array.Length; i++)
            {
                result += convertFunction(array[i]) + ",";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
            return result;
        }
        private static string ConvertArrayToString2<T>(in T[,] array, SerialyseFunction<T> convertFunction)
        {
            string result = "[";
            for (int l = 0; l < array.GetLength(0); l++)
            {
                result += "[";
                for (int c = 0; c < array.GetLength(1); c++)
                {
                    result += convertFunction(array[l, c]) + ",";
                }
                result = result.Remove(result.Length - 1, 1) + "]";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
            return result;
        }
        private static string ConvertArrayToString3<T>(in T[,,] array, SerialyseFunction<T> convertFunction)
        {
            string result = "";
            for (int l = 0; l < array.GetLength(0); l++)
            {
                result += "[";
                for (int c = 0; c < array.GetLength(1); c++)
                {
                    result += "[";
                    for (int i = 0; i < array.GetLength(2); i++)
                    {
                        result += convertFunction(array[l, c, i]) + ",";
                    }
                    result = result.Remove(result.Length - 1, 1) + "]";
                }
                result = result.Remove(result.Length - 1, 1) + "]";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
            return result;
        }
        private static string ConvertArrayToString4<T>(in T[,,,] array, SerialyseFunction<T> convertFunction)
        {
            string result = "";
            for (int l = 0; l < array.GetLength(0); l++)
            {
                result += "[";
                for (int c = 0; c < array.GetLength(1); c++)
                {
                    result += "[";
                    for (int i = 0; i < array.GetLength(2); i++)
                    {
                        result += "[";
                        for (int j = 0; j < array.GetLength(3); j++)
                        {
                            result += convertFunction(array[l, c, i, j]) + ",";
                        }
                        result = result.Remove(result.Length - 1, 1) + "]";
                    }
                    result = result.Remove(result.Length - 1, 1) + "]";
                }
                result = result.Remove(result.Length - 1, 1) + "]";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
            return result;
        }
        private static string ConvertArrayToString5<T>(in T[,,,,] array, SerialyseFunction<T> convertFunction)
        {
            string result = "";
            for (int l = 0; l < array.GetLength(0); l++)
            {
                result += "[";
                for (int c = 0; c < array.GetLength(1); c++)
                {
                    result += "[";
                    for (int i = 0; i < array.GetLength(2); i++)
                    {
                        result += "[";
                        for (int j = 0; j < array.GetLength(3); j++)
                        {
                            result += "[";
                            for (int k = 0; k < array.GetLength(4); k++)
                            {
                                result += convertFunction(array[l, c, i, j, k]) + ",";
                            }
                            result = result.Remove(result.Length - 1, 1) + "]";
                        }
                        result = result.Remove(result.Length - 1, 1) + "]";
                    }
                    result = result.Remove(result.Length - 1, 1) + "]";
                }
                result = result.Remove(result.Length - 1, 1) + "]";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
            return result;
        }
        #endregion

        public static float NearestFromZero(in float a, in float b) => Math.Abs(a) <= Math.Abs(b) ? a : b;
        public static float FarestFromZero(in float a, in float b) => Math.Abs(a) >= Math.Abs(b) ? a : b;

        #region Integrate

        public class Polynome
        {
            public float[] coeff;
            int deg
            {
                get => coeff.Length - 1;
            }

            public Polynome(float[] coeff)
            {
                this.coeff = coeff;
            }

            public float Evaluate(in float x)
            {
                float y = 0f;
                for (int i = 0; i < coeff.Length; i++)
                {
                    y += coeff[i] * MathF.Pow(x, i);
                }
                return y;
            }

            public float[] Evaluate(in float[] x)
            {
                float[] y = new float[x.Length];
                for (int i = 0; i < y.Length; i++)
                {
                    for (int k = 0; k < coeff.Length; k++)
                    {
                        y[i] += coeff[k] * MathF.Pow(x[i], k);
                    }
                }
                return y;
            }
        }

        public static float[][] MatrixDotProduct(in float[][] a, in float[][] b)
        {
            if (a[0].Length != b.Length)
                return null;

            float[][] ab = new float[a.Length][];
            for (int i = 0; i < ab.GetLength(0); i++)
            {
                ab[i] = new float[b[0].Length];
            }
            for (int i = 0; i < ab.GetLength(0); i++)
            {
                for (int j = 0; j < ab[i].Length; j++)
                {
                    float tmp = 0f;
                    for (int k = 0; k < a[0].Length; k++)
                    {
                        tmp += a[i][k] * b[k][j];
                    }
                    ab[i][j] = tmp;
                }
            }
            return ab;
        }

        public static T[][] MatrixTranspose<T>(in T[][] m)
        {
            T[][] mT = new T[m[0].Length][];
            for (int i = 0; i < mT.GetLength(0); i++)
            {
                mT[i] = new T[m.GetLength(0)];
            }
            for (int x = 0; x < m.GetLength(0); x++)
            {
                for (int y = 0; y < m[0].Length; y++)
                {
                    mT[y][x] = m[x][y];
                }
            }
            return mT;
        }

        /// <summary>
        /// Renvoie le PMA des points (p.x, p.y) de degré m < p.length
        /// </summary>
        /// <param name="p"></param>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static Polynome Regression(in float[] x, in float[] y, in int m)
        {
            if (x.Length != y.Length)
                return null;
            //Calculer v, v' et v'v
            float[][] v = new float[x.Length][];
            for (int i = 0; i < x.Length; i++)
            {
                v[i] = new float[m + 1];
            }
            for (int i = 0; i < x.Length; i++)
            {
                for (int j = 0; j <= m; j++)
                {
                    v[i][j] = MathF.Pow(x[i], j);
                }
            }

            float[][] vT = MatrixTranspose(v);
            float[][] vTv = MatrixDotProduct(vT, v);
            float[][] xMatrix = new float[x.Length][];
            float[][] yMatrix = new float[y.Length][];
            for (int i = 0; i < x.Length; i++)
            {
                xMatrix[i] = new float[1] { x[i] };
                yMatrix[i] = new float[1] { y[i] };
            }
            float[][] vTyTmp = MatrixDotProduct(vT, yMatrix);
            float[] vTy = new float[vTyTmp.GetLength(0)];
            for (int i = 0; i < vTy.Length; i++)
            {
                vTy[i] = vTyTmp[i][0];
            }
            //Résoudre l'équation v'va = v'y
            float[] a = MultipleRegression.NormalEquations<float>(vTv, vTy);

            return new Polynome(a);
        }

        /// <summary>
        /// Rerturn the integral between a and b of f(x)dx
        /// </summary>
        /// <param name="function">La function à intégré</param>
        /// <param name="a">le début de l'intégrale</param>
        /// <param name="b">la fin de l'intégrale</param>
        /// <param name="stepPerUnit">le nombre de point évalué par unité</param>
        /// <returns>The integral between a and b of f(x)dx</returns>
        public static float Integrate(function f, in float a, in float b, in float samplePerUnit = 5f)
        {
            if (a == b || samplePerUnit <= 0)
                return 0f;
            if (a > b)
                return -Integrate(f, b, a, samplePerUnit);
            float h = 1f / samplePerUnit;
            float end = b - (h / 2f);
            float I = 0f;//le résultat
            for (float ai = a; ai < end; ai += h)
            {
                I += f(ai) + 4f * f((2f * ai + h) / 2f) + f(ai + h);
            }
            return I * (h / 6f);
        }
        #endregion
    }
}