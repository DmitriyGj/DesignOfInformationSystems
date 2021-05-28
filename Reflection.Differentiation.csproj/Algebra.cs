using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Reflection.Differentiation
{
    public static class Algebra
    {
        public static Expression<Func<double,double>> Differentiate(Expression<Func<double,double>> func)
        {
            if(func is BinaryExpression)
            {
                var node = (BinaryExpression)func;
                if(node.Left is ParameterExpression)

            }
        }
    }   
}
