using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaeveFramework.Helpers.DynamicLinq
{
    public class SortFunc<T>
    {
        public Func<T, object> OrderByFunction { get; set; }

        public bool IsDesc { get; set; }
    }
}