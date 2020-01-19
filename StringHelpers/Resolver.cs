using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaeveFramework.StringHelpers
{
    public static class PropertiesResolver
    {
        /// <summary>
        /// Resolve properties from given object and populate given text with it's values. Useful for example to populate some message template
        /// </summary>
        /// <param name="text"></param>
        /// <param name="properties"></param>
        /// <param name="addSystemProps">Use default properties</param>
        /// <param name="openTag">Default: "{{"</param>
        /// <param name="closeTag">Default: "}}"</param>
        /// <param name="arraySeparator">Default: ", "</param>
        /// <returns>Text with resolved tags</returns>
        public static string Resolve(this string text, object properties, bool addSystemProps = true, string openTag = Consts.Helpers.TAG_OPEN, string closeTag = Consts.Helpers.TAG_CLOSE, string arraySeparator = Consts.Helpers.ARRAY_SEPARATOR)
        {
            var dict = GetPropertiesDict(properties, arraySeparator, addSystemProps);

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
        /// <param name="addSystemProps"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetPropertiesDict(object properties, string arraySeparator, bool addSystemProps)
        {
            var propertiesDict = new Dictionary<string, string>();
            if (addSystemProps)
            {
                propertiesDict.Add("$DATETIME", DateTime.Now.ToString(Consts.Helpers.DEFAULT_DATETIME_FORMAT));
                propertiesDict.Add("$HOSTNAME", Environment.MachineName);
            }

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
                    var classProps = GetPropertiesDict(value, arraySeparator, addSystemProps);
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
