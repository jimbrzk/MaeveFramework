using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MaeveFramework.Scheduler.Abstractions;

namespace MaeveFramework.Scheduler
{
    public class ScheduleString
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleString">START_DT|END_DT|DayOfWeek(1,7)|Repeat(TS)</param>
        /// <returns></returns>
        public static Schedule Parse(string scheduleString)
        {
            string[] scheduleEp = scheduleString.Split('|');

            List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();
            if (!String.IsNullOrWhiteSpace(scheduleEp[2]))
            {
                foreach (var ws in scheduleEp[2].Split(','))
                    dayOfWeeks.Add((DayOfWeek) Enum.Parse(typeof(DayOfWeek), ws));
            }

            TimeSpan repeat = TimeSpan.Zero;
            if(!String.IsNullOrWhiteSpace(scheduleEp[3]))
                repeat = TimeSpan.Parse(scheduleEp[3]);

            Schedule schedule = new Schedule(DateTime.Parse(scheduleEp[0]), DateTime.Parse(scheduleEp[1]), dayOfWeeks.ToArray(), repeat);

            return schedule;
        }

        public static string Parse(Schedule schedule)
        {
            string daysOfWeek = (schedule.DaysOfWeek != null) ? string.Join(",", schedule.DaysOfWeek) : string.Empty;

            return $"{schedule.Start}|{schedule.End}|{daysOfWeek}|{((schedule.Repeat.HasValue) ? schedule.Repeat.ToString() : string.Empty)}";
        }
    }
}
