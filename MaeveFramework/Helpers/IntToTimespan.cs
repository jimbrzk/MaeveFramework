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
        /// <summary>
        /// Create TimeSpan from miliseconds
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static TimeSpan Miliseconds(this int number) => TimeSpan.FromMilliseconds(number);
        /// <summary>
        /// Create TimeSpan from seconds
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static TimeSpan Seconds(this int number) => TimeSpan.FromSeconds(number);
        /// <summary>
        /// Create TimeSpan from minutes
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static TimeSpan Minutes(this int number) => TimeSpan.FromMinutes(number);
        /// <summary>
        /// Create TimeSpan from hours
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static TimeSpan Hours(this int number) => TimeSpan.FromHours(number);
        /// <summary>
        /// Create TimeSpan from Days
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static TimeSpan Days(this int number) => TimeSpan.FromDays(number);
        /// <summary>
        /// Create TimeSpan from weeks
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static TimeSpan Weeks(this int number) => TimeSpan.FromDays((number * 7));
    }
}
