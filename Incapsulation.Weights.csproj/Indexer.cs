using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Incapsulation.Weights
{
    public class Indexer
    {
        double[] pregrid;
        int length;
        int startIndex;
           
        public Indexer(double[] main, int start, int length)
        {
            Length = length;
            this.start = start;
            grid = main;
        }

        double[] grid
        {
            get => pregrid;
            set => pregrid = length + startIndex > value.Length || value is null ? throw new ArgumentException() : value;
        }

        public int Length
        {
            get => length;
            private set => length = value < 0 ? throw new ArgumentException() : value;
        }

        private int start
        {
            get => startIndex;
            set => startIndex = value < 0 ? throw new ArgumentException() : value;
        }

        public double this[int index]
        {
            get => index < 0 || startIndex + index > length ? throw new IndexOutOfRangeException(): grid[index+start];
            set => grid[index + start] = index < 0 || startIndex + index > length ? throw new IndexOutOfRangeException(): value;
        }
    }
}