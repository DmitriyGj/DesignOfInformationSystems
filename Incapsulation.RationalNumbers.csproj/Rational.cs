using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.RationalNumbers
{
    public class Rational 
    {
        public readonly int Numerator;
        public readonly int Denominator;
        public bool IsNan { get => Denominator == 0; }
        private bool IsInt { get => Numerator % Denominator == 0; }
        public Rational(int numerator, int denominator)
        {
            var divider = FindNOD(Math.Abs(numerator), denominator);
            if (divider != 0)
            {
                int sign = (numerator <0 && denominator < 0) || denominator < 0 ? -1 : 1;
                Numerator = numerator / divider * sign;
                Denominator = denominator / divider * sign;
            }
        }

        public Rational(int numerator)
        {
            this.Denominator = 1;
            this.Numerator = numerator;
        }

        public static Rational operator +(Rational a, Rational b)
        {
            if (!a.IsNan && !b.IsNan)
                return new Rational(a.Numerator * b.Denominator + b.Numerator * a.Denominator, 
                                    a.Denominator * b.Denominator);
            return new Rational(0, 0);
        }

        public static Rational operator -(Rational a, Rational b)
        {
            if (!a.IsNan && !b.IsNan)
                return new Rational(a.Numerator * b.Denominator - b.Numerator * a.Denominator,
                                    a.Denominator * b.Denominator);
            return new Rational(0, 0);
        }

        public static Rational operator *(Rational a, Rational b)
        {
            if (!a.IsNan && !b.IsNan)
                return new Rational(a.Numerator * b.Numerator, 
                                    a.Denominator * b.Denominator);
            return new Rational(0, 0);
        }

        public static Rational operator /(Rational a, Rational b)
        {
            if (b.Numerator != 0 && !b.IsNan)
                return new Rational(a.Numerator * b.Denominator,
                                    a.Denominator * b.Numerator);
            return new Rational(0, 0);
        }

        private int FindNOD(int num, int den)
        {
            while(den != 0)
            {
                var temp = num;
                num = den;
                den = temp % den;
            }
            return num;
        }

        public static implicit operator Rational(int number) => new Rational(number);
        public static implicit operator double(Rational number) => number.IsNan ? double.NaN: 
                                                  number.Numerator/(double)number.Denominator;
        public static explicit operator int(Rational number) => number.IsInt ? number.Numerator
                                                                : throw new ArgumentException() ;
    }
}
