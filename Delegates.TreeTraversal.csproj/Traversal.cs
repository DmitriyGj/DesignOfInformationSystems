using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.TreeTraversal
{
    public static class Traversal
    {
        public static IEnumerable<Product> GetProducts(ProductCategory root)
        {
        }

        public static IEnumerable<Job> GetEndJobs(Job root)
        {
            return GetNodes(root, item => item.Subjobs, item => item.Subjobs.Count == 0, job => job);
        }

        public static IEnumerable<T> GetBinaryTreeValues<T>(BinaryTree<T> root)
        {
            return GetNodes(root, item => new[] {item.Left, item.Right }.Where(node=> !(node is null)),
                item=>!(item.Left != null && item.Right != null),
                node => node.Value);
        }
        public static IEnumerable<Tresult> GetNodes<Tin, Tresult>
            (Tin MainRoot,
            Func<Tin, IEnumerable<Tin>> GetChildRoots, 
            Func<Tin, bool> PredicateToShoose, 
            Func<Tin, Tresult> NodeResultSelector)
        {
            if (PredicateToShoose(MainRoot))
                yield return NodeResultSelector(MainRoot);
            foreach (var nodes in GetChildRoots(MainRoot))
                foreach (var tranz_nodes in GetNodes(nodes, GetChildRoots, PredicateToShoose, NodeResultSelector))
                    yield return tranz_nodes;
        }
    }
}
