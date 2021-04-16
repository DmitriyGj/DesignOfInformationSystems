using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Generics.BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T>
        where T : IComparable
    {
        public BinaryTree<T> Left { get; set; }
        public BinaryTree<T> Right { get; set; }
        public T Value { get; set; }
        public bool IsRoot { get; set; } 

        public IEnumerator<T> GetEnumerator()
        {
            var way = Down(this);
            while (way.MoveNext())
                yield return way.Current;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerator<T> Down(BinaryTree<T> root)
        {
            if (!(root is null))
            {
                var leftWay = Down(root.Left);
                var rightWay = Down(root.Right);
                while (leftWay.MoveNext())
                    yield return leftWay.Current;
                if(root.IsRoot)
                    yield return root.Value;
                while (rightWay.MoveNext())
                    yield return rightWay.Current;
            }
        }
        public BinaryTree()
        {
            Left = null;
            Right = null;
            IsRoot = false;
        }

        public BinaryTree(T value)
        {
            IsRoot = true;
            Value = value;
            Left = null;
            Right = null;
        }

        public void Add(T value)
        {
            if (IsRoot)
            {
                if (Value.CompareTo(value) >= 0)
                {
                    if (Left is null)
                        Left = new BinaryTree<T>(value);
                    else
                        Left.Add(value);
                }
                if (Value.CompareTo(value) < 0)
                {
                    if (Right is null)
                        Right = new BinaryTree<T>(value);
                    else
                        Right.Add(value);
                }
            }
            else
            {
                Value = value;
                IsRoot = true;
            }

        }
    }
    public class BinaryTree
    {
        public static BinaryTree<T> Create<T>(params T[] values) where T : IComparable
        {
            BinaryTree<T> newTree = new BinaryTree<T>();
            foreach (T value in values)
                newTree.Add(value);
            return newTree;
        }
    }
}
