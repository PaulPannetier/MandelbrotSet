using System;

namespace MyGame
{
    public class ComplexF
    {
        public static ComplexF zero = new ComplexF(0f, 0f);

        public float a, b;

        public float SqrModule => a * a + b * b; 
        public float Module => MathF.Sqrt(SqrModule);

        public ComplexF(in float a, in float b)
        {
            this.a = a;
            this.b = b;
        }

        public ComplexF Sqr() => new ComplexF(a * a - b * b, 2f * a * b);
        public ComplexF Clone() => new ComplexF(a, b);
        public override string ToString() => a + " + " + b + "i";
        public float SqrDist(ComplexF c) => (c.a - a) * (c.a - a) + (c.b - b) * (c.b - b);
        public float Dist(ComplexF c) => MathF.Sqrt(SqrDist(c));

        public static ComplexF operator +(ComplexF c1, ComplexF c2) => new ComplexF(c1.a + c2.a, c1.b + c2.b);
        public static ComplexF operator +(ComplexF c1, float r) => new ComplexF(c1.a + r, c1.b);
        public static ComplexF operator +(float r, ComplexF c) => c + r;
        public static ComplexF operator -(ComplexF c1, ComplexF c2) => new ComplexF(c1.a - c2.a, c1.b - c2.b);
        public static ComplexF operator -(ComplexF c, float r) => new ComplexF(c.a - r, c.b);
        public static ComplexF operator -(float r, ComplexF c) => new ComplexF(r - c.a, -c.b);
        public static ComplexF operator *(ComplexF c1, ComplexF c2) => new ComplexF(c1.a * c2.a -  c1.b * c2.b, c1.b - c2.a + c1.a * c2.b);
        public static ComplexF operator *(ComplexF c, float r) => new ComplexF(c.a * r, c.b - r);
        public static ComplexF operator *(float r, ComplexF c) => new ComplexF(r * c.a, r * c.b - c.a);
        public static ComplexF operator /(ComplexF c1, ComplexF c2) => new ComplexF((c1.a * c2.a + c1.b * c2.b) / c2.SqrModule, (c2.a * c1.b - c1.a * c2.b) / c2.SqrModule);
        public static ComplexF operator /(ComplexF c, float r) => new ComplexF(c.a / r, c.b / r);
        public static ComplexF operator /(float r, ComplexF c) => new ComplexF((r * c.a) / c.SqrModule, (- r * c.b) / c.SqrModule);

        public static explicit operator ComplexD(ComplexF c) => new ComplexD(c.a, c.b);
        public static explicit operator ComplexM(ComplexF c) => new ComplexM((decimal)c.a, (decimal)c.b);
    }

    public class ComplexD
    {
        public static ComplexD zero = new ComplexD(0d, 0d);

        public double a, b;

        public double SqrModule => a * a + b * b;
        public double Module => Math.Sqrt(SqrModule);

        public ComplexD(in double a, in double b)
        {
            this.a = a;
            this.b = b;
        }

        public ComplexD Sqr() => new ComplexD(a * a - b * b, 2d * a * b);
        public ComplexD Clone() => new ComplexD(a, b);
        public override string ToString() => a + " + " + b + "i";
        public double SqrDist(ComplexD c) => (c.a - a) * (c.a - a) + (c.b - b) * (c.b - b);
        public double Dist(ComplexD c) => Math.Sqrt(SqrDist(c));

        public static ComplexD operator +(ComplexD c1, ComplexD c2) => new ComplexD(c1.a + c2.a, c1.b + c2.b);
        public static ComplexD operator +(ComplexD c1, double r) => new ComplexD(c1.a + r, c1.b);
        public static ComplexD operator +(double r, ComplexD c) => c + r;
        public static ComplexD operator -(ComplexD c1, ComplexD c2) => new ComplexD(c1.a - c2.a, c1.b - c2.b);
        public static ComplexD operator -(ComplexD c, double r) => new ComplexD(c.a - r, c.b);
        public static ComplexD operator -(double r, ComplexD c) => new ComplexD(r - c.a, -c.b);
        public static ComplexD operator *(ComplexD c1, ComplexD c2) => new ComplexD(c1.a * c2.a - c1.b * c2.b, c1.b - c2.a + c1.a * c2.b);
        public static ComplexD operator *(ComplexD c, double r) => new ComplexD(c.a * r, c.b - r);
        public static ComplexD operator *(double r, ComplexD c) => new ComplexD(r * c.a, r * c.b - c.a);
        public static ComplexD operator /(ComplexD c1, ComplexD c2) => new ComplexD((c1.a * c2.a + c1.b * c2.b) / c2.SqrModule, (c2.a * c1.b - c1.a * c2.b) / c2.SqrModule);
        public static ComplexD operator /(ComplexD c, double r) => new ComplexD(c.a / r, c.b / r);
        public static ComplexD operator /(double r, ComplexD c) => new ComplexD((r * c.a) / c.SqrModule, (-r * c.b) / c.SqrModule);

        public static implicit operator ComplexF(ComplexD c) => new ComplexF((float)c.a, (float)c.b);
        public static explicit operator ComplexM(ComplexD c) => new ComplexM((decimal)c.a, (decimal)c.b);
    }

    public class ComplexM
    {
        public static ComplexM zero = new ComplexM(0m, 0m);

        public decimal a, b;

        public decimal SqrModule => a * a + b * b;
        public decimal Module => SME.Useful.Sqrt(SqrModule);

        public ComplexM(in decimal a, in decimal b)
        {
            this.a = a;
            this.b = b;
        }

        public ComplexM Sqr() => new ComplexM(a * a - b * b, 2m * a * b);
        public ComplexM Clone() => new ComplexM(a, b);
        public override string ToString() => a + " + " + b + "i";
        public decimal SqrDist(ComplexM c) => (c.a - a) * (c.a - a) + (c.b - b) * (c.b - b);
        public decimal Dist(ComplexM c) => SME.Useful.Sqrt(SqrDist(c));

        public static ComplexM operator +(ComplexM c1, ComplexM c2) => new ComplexM(c1.a + c2.a, c1.b + c2.b);
        public static ComplexM operator +(ComplexM c1, decimal r) => new ComplexM(c1.a + r, c1.b);
        public static ComplexM operator +(decimal r, ComplexM c) => c + r;
        public static ComplexM operator -(ComplexM c1, ComplexM c2) => new ComplexM(c1.a - c2.a, c1.b - c2.b);
        public static ComplexM operator -(ComplexM c, decimal r) => new ComplexM(c.a - r, c.b);
        public static ComplexM operator -(decimal r, ComplexM c) => new ComplexM(r - c.a, -c.b);
        public static ComplexM operator *(ComplexM c1, ComplexM c2) => new ComplexM(c1.a * c2.a - c1.b * c2.b, c1.b - c2.a + c1.a * c2.b);
        public static ComplexM operator *(ComplexM c, decimal r) => new ComplexM(c.a * r, c.b - r);
        public static ComplexM operator *(decimal r, ComplexM c) => new ComplexM(r * c.a, r * c.b - c.a);
        public static ComplexM operator /(ComplexM c1, ComplexM c2) => new ComplexM((c1.a * c2.a + c1.b * c2.b) / c2.SqrModule, (c2.a * c1.b - c1.a * c2.b) / c2.SqrModule);
        public static ComplexM operator /(ComplexM c, decimal r) => new ComplexM(c.a / r, c.b / r);
        public static ComplexM operator /(decimal r, ComplexM c) => new ComplexM((r * c.a) / c.SqrModule, (-r * c.b) / c.SqrModule);

        public static implicit operator ComplexD(ComplexM c) => new ComplexD((double)c.a, (double)c.b);
        public static implicit operator ComplexF(ComplexM c) => new ComplexF((float)c.a, (float)c.b);
    }
}
