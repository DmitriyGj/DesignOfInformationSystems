using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.PairsAnalysis
{
    public static class Analysis
    {
        public static int FindMaxPeriodIndex(params DateTime[] data) =>
            data.Pairs().Select(pair => (pair.Item2 - pair.Item1).TotalSeconds).MaxIndex();

        public static double FindAverageRelativeDifference(params double[] data) =>
            data.Pairs().Select(pair => (pair.Item2 - pair.Item1) / pair.Item1).Average();

        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> a)
        {
            var b = a.GetEnumerator();
            b.MoveNext();
            var previous = b.Current;
            while (b.MoveNext())
            {
                yield return Tuple.Create(previous, b.Current);
                previous = b.Current;
            }
        }

        public static int MaxIndex<T>(this IEnumerable<T> a)=>
            a.Select((value, index) => Tuple.Create(value, index))
                         .OrderByDescending(tuple => tuple.Item1)
                         .ToArray().First().Item2;
    }
}