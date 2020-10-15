using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MaeveFramework.Helpers
{
    /// <summary>
    /// Converts Int32 to TimeSpan
    /// </summary>
    public static class IntToTimespan
    {
        public static TimeSpan Miliseconds(this int number) => TimeSpan.FromMilliseconds(number);
        public static TimeSpan Seconds(this int number) => TimeSpan.FromSeconds(number);
        public static TimeSpan Minutes(this int number) => TimeSpan.FromMinutes(number);
        public static TimeSpan Hours(this int number) => TimeSpan.FromHours(number);
        public static TimeSpan Days(this int number) => TimeSpan.FromDays(number);
        public static TimeSpan Weeks(this int number) => TimeSpan.FromDays((number * 7));
    }
}
