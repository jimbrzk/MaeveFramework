using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MaeveFramework.Scheduler.Abstractions;

namespace MaeveFramework.Scheduler
{
    /// <summary>
    /// Schedule parser
    /// </summary>
    public class ScheduleString
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleString">START_DT|END_DT|DayOfWeek(1,7)|DaysOfMonths(1,31)|Repeat(TS)|Never</param>
        /// <returns></returns>
        public static Schedule Parse(string scheduleString)
        {
            if (String.IsNullOrWhiteSpace(scheduleString))
                return new Schedule();
            else if (scheduleString.ToLower() == "|||||true")
                return new Schedule(never: true);

            string[] scheduleEp = scheduleString.Split('|');

            TimeSpan? start = null;
            if (scheduleEp.Length > 0 && !String.IsNullOrWhiteSpace(scheduleEp[0]))
                start = TimeSpan.Parse(scheduleEp[0]);

            TimeSpan? end = null;
            if (scheduleEp.Length > 1 && !String.IsNullOrWhiteSpace(scheduleEp[1]))
                end = TimeSpan.Parse(scheduleEp[1]);

            List<DayOfWeek> dayOfWeeks = new List<DayOfWeek>();
            if (scheduleEp.Length > 2 && !String.IsNullOrWhiteSpace(scheduleEp[2]))
            {
                foreach (var ws in scheduleEp[2].Split(','))
                    dayOfWeeks.Add((DayOfWeek) Enum.Parse(typeof(DayOfWeek), ws));
            }
            List<int> daysOfMonth = new List<int>();
            if (scheduleEp.Length > 3 && !String.IsNullOrWhiteSpace(scheduleEp[3]))
            {
                foreach (var ws in scheduleEp[3].Split(','))
                    daysOfMonth.Add(int.Parse(ws));
            }

            TimeSpan? repeat = null;
            if(scheduleEp.Length > 4 && !String.IsNullOrWhiteSpace(scheduleEp[4]))
                repeat = TimeSpan.Parse(scheduleEp[4]);

            bool never = false;
            if (scheduleEp.Length > 5 && !String.IsNullOrWhiteSpace(scheduleEp[5]))
                never = bool.Parse(scheduleEp[5]);


            return new Schedule(start, end, dayOfWeeks.ToArray(), daysOfMonth.ToArray(), repeat, never);
        }

        /// <summary>
        /// Create schedule string representation
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public static string Parse(Schedule schedule)
        {
            if (schedule.Never)
                return "|||||True";
            else if (schedule.Always)
                return string.Empty;

            string daysOfWeek = (schedule.DaysOfWeek != null) ? string.Join(",", schedule.DaysOfWeek) : string.Empty;
            string daysOfMonth = (schedule.DaysOfMonth != null) ? string.Join(",", schedule.DaysOfMonth) : string.Empty;

            return $"{schedule.Start}|{schedule.End}|{daysOfWeek}|{daysOfMonth}|{((schedule.Repeat.HasValue) ? schedule.Repeat.ToString() : string.Empty)}|{schedule.Never}";
        }
    }
}
