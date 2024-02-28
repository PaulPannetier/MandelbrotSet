using System;
using Microsoft.Xna.Framework;

namespace SME
{
    public delegate float function(in float t);//fonction de [0, 1] => |R avec f(0) = 0 et f(1) = 1

    public class Tweener
    {
        private float duration, beginValue, currentValue, timer = 0f, deltay;
        private function tweeningFunction;
        public bool isFinish = false;
        public float durationPoucentage
        {
            get => timer / duration;
        }

        public Tweener(float beginValue, float endValue, float duration, function tweeningFunction)
        {
            this.duration = duration; this.beginValue = beginValue; this.tweeningFunction = tweeningFunction;
            this.currentValue = beginValue;
            deltay = endValue - beginValue;
        }        

        public float GetValue => currentValue;

        public void Reset()
        {
            timer = 0f;
            isFinish = false;
            currentValue = beginValue;
        }

        public void Update()
        {
            timer += Time.dt;
            if(timer >= duration)
            {
                timer = duration;
                isFinish = true;
            }
            currentValue = beginValue + tweeningFunction(timer/duration) * deltay;
        }
    }

    public class ColorTweener
    {
        private Color beginColor, endColor;
        public Color GetColor => new Color((byte)tweenerR.GetValue, (byte)tweenerG.GetValue, (byte)tweenerB.GetValue, (byte)tweenerA.GetValue);
        public bool isFinish => tweenerR.isFinish;
        public float durationPourcentage
        {
            get => tweenerA.durationPoucentage;
        }
        private Tweener tweenerR, tweenerG, tweenerB, tweenerA;

        public ColorTweener(Color beginColor, Color endColor, function tweeningFonction, float duration)
        {
            this.beginColor = beginColor;
            this.endColor = endColor;
            this.tweenerR = new Tweener(this.beginColor.R, this.endColor.R, duration, tweeningFonction);
            this.tweenerG = new Tweener(this.beginColor.G, this.endColor.G, duration, tweeningFonction);
            this.tweenerB = new Tweener(this.beginColor.B, this.endColor.B, duration, tweeningFonction);
            this.tweenerA = new Tweener(this.beginColor.A, this.endColor.A, duration, tweeningFonction);
        }

        public void Reset()
        {
            tweenerR.Reset();
            tweenerG.Reset();
            tweenerB.Reset();
            tweenerA.Reset();
        }

        public void Update()
        {
            tweenerR.Update();
            tweenerG.Update();
            tweenerB.Update();
            tweenerA.Update();
        }
    }

    public static class TweeningFunction
    {
        public static float Sin1(in float t) => (float)Math.Sin(t * MathHelper.PiOver2);
        public static float Sin2(in float t) => (float)Math.Sin(t * MathHelper.PiOver2 * 5d);
        public static float Linear(in float t) => t;
        public static float Sqrt(in float t) => (float)Math.Sqrt(t);
        public static float Square(in float t) => t * t;
        public static float Polinomial3(in float t) => MathF.Pow(t, 3);
        public static float Polinomial4(in float t) => MathF.Pow(t, 4);
        public static float Polinomial5(in float t) => MathF.Pow(t, 5);
        public static float Polinomial6(in float t) => MathF.Pow(t, 6);
        public static float Polinomial7(in float t) => MathF.Pow(t, 7);
        public static float Polinomial8(in float t) => MathF.Pow(t, 8);
        /// <summary>
        /// return x^a / (x^a - (b(1 - x))^a)
        /// </summary>
        public static float SinLog(in float x, in float a = 3f, in float b = 2.2f) => MathF.Pow(x, a) / (MathF.Pow(x, a) - MathF.Pow(b - b * x, a));
    }

    public static class EasingFunctions
    {
        public static float Linear(float value) => value;

        public static float CubicIn(float value) => Power.In(value, 3);
        public static float CubicOut(float value) => Power.Out(value, 3);
        public static float CubicInOut(float value) => Power.InOut(value, 3);

        public static float QuadraticIn(float value) => Power.In(value, 2);
        public static float QuadraticOut(float value) => Power.Out(value, 2);
        public static float QuadraticInOut(float value) => Power.InOut(value, 2);

        public static float QuarticIn(float value) => Power.In(value, 4);
        public static float QuarticOut(float value) => Power.Out(value, 4);
        public static float QuarticInOut(float value) => Power.InOut(value, 4);

        public static float QuinticIn(float value) => Power.In(value, 5);
        public static float QuinticOut(float value) => Power.Out(value, 5);
        public static float QuinticInOut(float value) => Power.InOut(value, 5);

        public static float SineIn(float value) => (float)Math.Sin(value * MathHelper.PiOver2 - MathHelper.PiOver2) + 1;
        public static float SineOut(float value) => (float)Math.Sin(value * MathHelper.PiOver2);
        public static float SineInOut(float value) => (float)(Math.Sin(value * MathHelper.Pi - MathHelper.PiOver2) + 1) / 2;

        public static float ExponentialIn(float value) => (float)Math.Pow(2, 10 * (value - 1));
        public static float ExponentialOut(float value) => Out(value, ExponentialIn);
        public static float ExponentialInOut(float value) => InOut(value, ExponentialIn);

        public static float CircleIn(float value) => (float)-(Math.Sqrt(1 - value * value) - 1);
        public static float CircleOut(float value) => (float)Math.Sqrt(1 - (value - 1) * (value - 1));
        public static float CircleInOut(float value) => (float)(value <= .5 ? (Math.Sqrt(1 - value * value * 4) - 1) / -2 : (Math.Sqrt(1 - (value * 2 - 2) * (value * 2 - 2)) + 1) / 2);

        public static float ElasticIn(float value)
        {
            const int oscillations = 1;
            const float springiness = 3f;
            var e = (Math.Exp(springiness * value) - 1) / (Math.Exp(springiness) - 1);
            return (float)(e * Math.Sin((MathHelper.PiOver2 + MathHelper.TwoPi * oscillations) * value));
        }

        public static float ElasticOut(float value) => Out(value, ElasticIn);
        public static float ElasticInOut(float value) => InOut(value, ElasticIn);

        public static float BackIn(float value)
        {
            const float amplitude = 1f;
            return (float)(Math.Pow(value, 3) - value * amplitude * Math.Sin(value * MathHelper.Pi));
        }

        public static float BackOut(float value) => Out(value, BackIn);
        public static float BackInOut(float value) => InOut(value, BackIn);

        public static float BounceOut(float value) => Out(value, BounceIn);
        public static float BounceInOut(float value) => InOut(value, BounceIn);

        public static float BounceIn(float value)
        {
            const float bounceConst1 = 2.75f;
            var bounceConst2 = (float)Math.Pow(bounceConst1, 2);

            value = 1 - value; //flip x-axis

            if (value < 1 / bounceConst1) // big bounce
                return 1f - bounceConst2 * value * value;

            if (value < 2 / bounceConst1)
                return 1 - (float)(bounceConst2 * Math.Pow(value - 1.5f / bounceConst1, 2) + .75);

            if (value < 2.5 / bounceConst1)
                return 1 - (float)(bounceConst2 * Math.Pow(value - 2.25f / bounceConst1, 2) + .9375);

            //small bounce
            return 1f - (float)(bounceConst2 * Math.Pow(value - 2.625f / bounceConst1, 2) + .984375);
        }


        private static float Out(float value, Func<float, float> function)
        {
            return 1 - function(1 - value);
        }

        private static float InOut(float value, Func<float, float> function)
        {
            if (value < 0.5f)
                return 0.5f * function(value * 2);

            return 1f - 0.5f * function(2 - value * 2);
        }

        private static class Power
        {
            public static float In(double value, int power)
            {
                return (float)Math.Pow(value, power);
            }

            public static float Out(double value, int power)
            {
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign * (Math.Pow(value - 1, power) + sign));
            }

            public static float InOut(double s, int power)
            {
                s *= 2;

                if (s < 1)
                    return In(s, power) / 2;

                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (Math.Pow(s - 2, power) + sign * 2));
            }
        }
    }
}