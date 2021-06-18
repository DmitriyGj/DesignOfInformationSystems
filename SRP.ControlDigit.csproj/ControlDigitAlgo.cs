using System;
using System.Collections.Generic;
using System.Linq;

namespace SRP.ControlDigit
{
    public static class Extensions
    {
        public static List<int> ToDigits(this long number)
        {
            var result = new List<int>();
            while(number > 0)
            {
                result.Add((int)(number % 10)); 
                number /= 10;
            }
            return result;
        }

        public static int CalcProgression(this List<int> digits,int factor,int coefficient, bool dash)
        {
            int result = 0;
            foreach (var digit in digits)
            {
                result += factor * digit;
                factor = coefficient + (dash ? -1:1)* factor;
            }
            return result;
        }
        
    }

    public static class ControlDigitAlgo
    {
        public static int Upc(long number)
        {
            int sum = number.ToDigits().CalcProgression(3,4,true);
            int result = sum % 10;
            if (result != 0)
                result = 10 - result;
            return result;
        }

        public static int Isbn10(long number)
        {
            var sum = number.ToDigits().CalcProgression(2,1,false);
            var result = sum % 11 == 0  ? sum : 10 - (sum % 11 - 1);
            return result == 10 ? 'X' : result.ToString().First();
        }

        public static int Luhn(long number) => number.ToDigits().CalcProgression(1,4,true);
    }
}
