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
        int length;
        int startIndex;
        double[] preGrid;
        double[] grid
        {
            get => preGrid;
            set => preGrid = length + startIndex > value.Length ? throw new ArgumentException() : value;
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

        public Indexer(double[] neiro, int start, int length)
        {
            Length = length;
            this.start = start;
            grid = neiro;
        }

        public double this[int index]
        {
            get=> index< 0 || start + index> Length ? throw new IndexOutOfRangeException() : grid[index + start];
            set=> grid[index + start] = index < 0 || index + start > Length ? 
                  throw new IndexOutOfRangeException() : value;
        }
    }
}