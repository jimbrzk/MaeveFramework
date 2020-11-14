using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Linq.Dynamic;

namespace MaeveFramework.Helpers.DynamicLinq
{
    //Wrapper dla nuggetowskiej bilbioteki do parsowania string na lambdy
    public static class PredicateHelper
    {
        public static Expression<TDelegate> AndAlso<TDelegate>(this Expression<TDelegate> left, Expression<TDelegate> right)
        {
            return Expression.Lambda<TDelegate>(Expression.AndAlso(left, right), left.Parameters);
        }

        public static Expression<Func<T, bool>> CreateFromString<T>(string where)
        {
                return (Expression<Func<T, bool>>) System.Linq.Dynamic.Core.DynamicExpressionParser.ParseLambda(typeof(T), typeof(bool), where);
        }
    }
}