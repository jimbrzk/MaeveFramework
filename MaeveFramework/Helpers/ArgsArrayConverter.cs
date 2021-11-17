using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaeveFramework.Helpers
{
    /// <summary>
    /// Convert arguments in array to dictionary
    /// </summary>
    public static class ArgsArrayConverter
    {
        /// <summary>
        /// Convert array to dict
        /// </summary>
        /// <param name="args"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ArgsToDictionary(this string[] args, string prefix = "-")
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if (args[i].StartsWith(prefix))
                    {
                        dict.Add(args[i].Remove(0, 1).ToLower(), (args.Length > (i + 1)) ? args[(i + 1)] : string.Empty);
                    }
                }
                catch (Exception) { }
            }
            return dict;
        }
    }
}
