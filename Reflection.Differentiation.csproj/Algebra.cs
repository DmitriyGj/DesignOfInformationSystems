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
        
        static Expression GetDifferentiate(Expression func)
        {
            switch (func.NodeType)
            {
                case ExpressionType.Constant:return Expression.Constant(0.0);
                case ExpressionType.Parameter:return Expression.Constant(1.0);
                case ExpressionType.Call: return GetDifferentiate(func as MethodCallExpression);
                case ExpressionType.Multiply:return CalculateDifferentiationMultiply(func as BinaryExpression);
                case ExpressionType.Add :return CalculateDifferentiationAdd(func as BinaryExpression);
                default:throw new ArgumentException(func.ToString());
            }
        }

        static Expression GetDifferentiate(MethodCallExpression func)
        {
            switch (func.Method.Name )
            {
                case nameof(Math.Cos): return CalculateDifferentiateCos(func);
                case nameof(Math.Sin): return CalculateDifferentiationSin(func);
                default: throw new ArgumentException(func.ToString());
            }
        }
        
        static Expression CalculateDifferentiationSin(MethodCallExpression func)=>
                        Expression.Multiply(Expression.Call(typeof(Math).GetMethod("Cos"),func.Arguments.First()), 
                       GetDifferentiate(func.Arguments.First()));
        
        static Expression CalculateDifferentiateCos(MethodCallExpression func)=>Expression.Multiply(
                Expression.Multiply(Expression.Call(typeof(Math).GetMethod("Sin"),
                                func.Arguments.First()),Expression.Constant(-1.0)),
                GetDifferentiate(func.Arguments.First()));

        static Expression CalculateDifferentiationAdd(BinaryExpression func) =>
            Expression.Add(GetDifferentiate(func.Left), GetDifferentiate(func.Right));

        static Expression CalculateDifferentiationMultiply(BinaryExpression func) => Expression.Add(
            Expression.Multiply(GetDifferentiate(func.Left), func.Right),
            Expression.Multiply(GetDifferentiate(func.Right), func.Left));
    }   
}
