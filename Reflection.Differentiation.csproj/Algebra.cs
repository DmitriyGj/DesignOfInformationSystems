using System;
using System.Linq;
using System.Linq.Expressions;

namespace Reflection.Differentiation
{
    public static class Algebra
    {
        public static Expression<Func<double,double>> Differentiate(Expression<Func<double,double>> func)
        {
            var param = func.Parameters;
            var result = GetDifferentiate(func.Body);
            return Expression.Lambda<Func<double,double>>(result,param);
        }

        public static Expression GetDifferentiate(Expression func)
        {
            var nodeType = func.NodeType;
            if (nodeType is ExpressionType.Constant)
                return Expression.Constant(0.0);
            if (nodeType is ExpressionType.Parameter)
                return Expression.Constant(1.0);
            if (nodeType is ExpressionType.Add)
            {
                var left = GetDifferentiate((func as BinaryExpression).Left);
                var right = GetDifferentiate((func as BinaryExpression).Right);
                return Expression.Add(left, right);
            }

            if (nodeType is ExpressionType.Multiply)
            {
                var likeBinary = func as BinaryExpression;
                var left = GetDifferentiate(likeBinary.Left);
                var right = GetDifferentiate(likeBinary.Right);
                return Expression.Add(Expression.Multiply(left, likeBinary.Right),
                    Expression.Multiply(right, likeBinary.Left));
            }

            if (nodeType is ExpressionType.Call)
            {
                var call = (func as MethodCallExpression);
                var body = call.Arguments.First();
                switch (call.Method.Name )
                {
                    case "Cos":
                        return Expression.Multiply(Expression.Multiply(Expression.Constant(-1.0),  
                                Expression.Call(typeof(Math).GetMethod("Sin"),body)),
                            GetDifferentiate(body));
                    case "Sin":
                        return Expression.Multiply(Expression.Call(typeof(Math).GetMethod("Cos"),body),
                        GetDifferentiate(body));
                }
            }
            throw new ArgumentException(func.ToString());
        }
    }   
}
