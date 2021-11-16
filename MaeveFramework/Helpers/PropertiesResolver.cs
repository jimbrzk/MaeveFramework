using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaeveFramework.Helpers
{
    /// <summary>
    /// Helping class for preoperties resolving. For example, you can use it for email templating
    /// </summary>
    public static class PropertiesResolver
    {
        /// <summary>
        /// Resolve properties from given object and populate given text with it's values. Useful for example to populate some message template
        /// </summary>
        /// <param name="text"></param>
        /// <param name="properties"></param>
        /// <param name="openTag">Default: "{{"</param>
        /// <param name="closeTag">Default: "}}"</param>
        /// <param name="arraySeparator">Default: ", "</param>
        /// <returns>Text with resolved tags</returns>
        public static string Resolve(this string text, object properties, string openTag = Consts.Helpers.TAG_OPEN, string closeTag = Consts.Helpers.TAG_CLOSE, string arraySeparator = Consts.Helpers.ARRAY_SEPARATOR)
        {
            var dict = GetPropertiesDict(properties, arraySeparator);

            foreach (var prop in dict)
            {
                text = text.Replace(Consts.Helpers.TAG_OPEN + prop.Key + Consts.Helpers.TAG_CLOSE, prop.Value);
            }

            return text;
        }

        /// <summary>
        /// Resolve properties from given Dictionary and populate given text with it's values. Useful for example to populate some message template
        /// </summary>
        /// <param name="text"></param>
        /// <param name="dict"></param>
        /// <param name="openTag">Default: "{{"</param>
        /// <param name="closeTag">Default: "}}"</param>
        /// <returns></returns>
        public static string Resolve(this string text, Dictionary<string, string> dict, string openTag = Consts.Helpers.TAG_OPEN, string closeTag = Consts.Helpers.TAG_CLOSE)
        {
            foreach (var prop in dict)
            {
                text = text.Replace(Consts.Helpers.TAG_OPEN + prop.Key + Consts.Helpers.TAG_CLOSE, prop.Value);
            }

            return text;
        }

        /// <summary>
        /// Returning properties dictionary from given object 
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="arraySeparator"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetPropertiesDict(object properties, string arraySeparator)
        {
            var propertiesDict = new Dictionary<string, string>();

            var props = properties.GetType().GetProperties();

            foreach (var prop in props)
            {
                var value = prop.GetValue(properties);
                if(value == null)
                    continue;

                Type type = value.GetType();

                if (type.IsArray)
                {
                    propertiesDict.Add(prop.Name, String.Join(arraySeparator, value));
                }
                else if (type.IsEnum)
                {
                    propertiesDict.Add(prop.Name, Enum.GetName(type, value));
                }
                else if (type.IsClass)
                {
                    var classProps = GetPropertiesDict(value, arraySeparator);
                    if (classProps.Count > 0)
                       propertiesDict = propertiesDict.Concat(classProps).ToDictionary(k => k.Key, v => v.Value);
                }
                else
                {
                    propertiesDict.Add(prop.Name, value.ToString());
                }
            }

            return propertiesDict;
        }
    }
}
