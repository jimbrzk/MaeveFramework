using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MaeveFramework.Helpers.DynamicLinq
{
    /// <summary>
    /// Ta klasa sluzy Ci do konwersji stringa na predykaty sortujace. Zapisz sobie to
    /// Bo to naprawde kawal fajnego kodu ;-)
    /// </summary>
    public static class SortFunctionHelper
    {
        // Code from: https://www.red-gate.com/simple-talk/dotnet/net-framework/dynamic-linq-queries-with-expression-trees/
        // Fixes from: https://stackoverflow.com/questions/31955025/generate-ef-orderby-expression-by-string
        public static SortFunc<T>[] CreateFromString<T>(string orderBy)
        {
            var propertiesNames = orderBy.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            var navigation = new List<SortFunc<T>>();
            var type = typeof(T);

            foreach (var propertyName in propertiesNames)
            {
                var cleanPropertyName = propertyName;
                var isDesc = propertyName.Contains("_desc");

                if (isDesc)
                    cleanPropertyName = propertyName.Replace("_desc", "");

                cleanPropertyName = cleanPropertyName.Trim();
                var parameter = Expression.Parameter(type);
                Expression propertyReference = Expression.Property(parameter,
                    cleanPropertyName);

                if (propertyReference.Type.IsValueType)
                    propertyReference = Expression.Convert(propertyReference, typeof(object));

                navigation.Add(new SortFunc<T>
                {
                    OrderByFunction = Expression.Lambda<Func<T, object>>
                        (propertyReference, parameter).Compile(),
                    IsDesc = isDesc
                });
            }

            return navigation.AsEnumerable()
                .Reverse()
                .ToArray();
        }
    }
}