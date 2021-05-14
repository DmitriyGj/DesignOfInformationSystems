using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.TreeTraversal
{
    public static class Traversal
    {
<<<<<<< HEAD
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
=======
        public static IEnumerable<Product> GetProducts(ProductCategory root)=>
                Detour<ProductCategory, IEnumerable<Product>>(root, 
                category => category.Categories, 
                categoey => categoey.Products.Count > 0, 
                category => category.Products).SelectMany(prod=>prod);

        public static IEnumerable<Job> GetEndJobs(Job root)=>
                Detour<Job, Job>(root, 
                job => job.Subjobs, 
                job => job.Subjobs.Count == 0, 
                job => job);

        public static IEnumerable<T> GetBinaryTreeValues<T>(BinaryTree<T> root)=>
                Detour<BinaryTree<T>,T>(root,
                node=> new[] { node.Left, node.Right }.Where(value=>value != null), 
                (child) => child.Left is null && child.Right is null, 
                node => node.Value); 

        public static IEnumerable<TResult> Detour<TIn, TResult>(TIn main_root, 
            Func<TIn, IEnumerable<TIn>> get_child, 
            Func<TIn, bool> predicate, 
            Func<TIn, TResult> result_selector)
        {
            if (predicate(main_root))
                yield return result_selector(main_root);
            foreach(var child in get_child(main_root))
                foreach(var item in Detour(child, get_child, predicate, result_selector))
                    yield return item;
>>>>>>> 648966bea90d11772e2a96eeb6b4f702c2667a03
        }
    }
}
