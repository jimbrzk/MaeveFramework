using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework
{
    /// <summary>
    /// Consts
    /// </summary>
    public class Consts
    {
        /// <summary>
        /// Helpers consts
        /// </summary>
        public class Helpers
        {
            // StringHelpers
            /// <summary>
            /// Default open tag for string helper
            /// </summary>
            public const string TAG_OPEN = "{{";
            /// <summary>
            /// Default close tag for string helper
            /// </summary>
            public const string TAG_CLOSE = "}}";
            /// <summary>
            /// Default array separator tag for string helper
            /// </summary>
            public const string ARRAY_SEPARATOR = ", ";

            // Others
            /// <summary>
            /// Default datetime string
            /// </summary>
            public const string DEFAULT_DATETIME_FORMAT = "dd-MM-yyyy HH:mm:ss";
        }
    }
}
