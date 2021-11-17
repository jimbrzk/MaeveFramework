using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaeveFramework.Scheduler.Abstractions
{
    /// <summary>
    /// Schedule class
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// Start of job date time
        /// </summary>
        public readonly TimeSpan? Start;
        /// <summary>
        /// Job end date time
        /// </summary>
        public readonly TimeSpan? End;
        /// <summary>
        /// Days of week when job can be run. Every day when null
        /// </summary>
        public readonly DayOfWeek[] DaysOfWeek;
        /// <summary>
        /// Run only on specific days in month
        /// </summary>
        public readonly int[] DaysOfMonth;
        /// <summary>
        /// Repeat timespan. Always running when null
        /// </summary>
        public readonly TimeSpan? Repeat;
        /// <summary>
        /// Job will not be executed
        /// </summary>
        public readonly bool Never;
        /// <summary>
        /// Return true when Never is false and no value has bean seat for Repeat, DaysOfWeek, DaysOfMonth
        /// </summary>
        public bool Always => (!Never && !Repeat.HasValue && DaysOfWeek.Length == 0 && DaysOfMonth.Length == 0);

        private readonly MaeveFramework.Logger.Abstractions.ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start">Start schedule from</param>
        /// <param name="end">End schedule at</param>
        /// <param name="daysOfWeek">Days of week for schedule</param>
        /// <param name="daysOfMonth">Days of month for shcedule</param>
        /// <param name="repeat">Repeat</param>
        /// <param name="never">Never</param>
        public Schedule(TimeSpan? start = null, TimeSpan? end = null, DayOfWeek[] daysOfWeek = null, int[] daysOfMonth = null, TimeSpan? repeat = null, bool never = false)
        {
            _logger = Logger.LoggingManager.GetLogger(nameof(Schedule));

            Start = start;
            End = end;
            DaysOfWeek = daysOfWeek;
            DaysOfMonth = daysOfMonth;
            Repeat = repeat;
            Never = never;
        }

        /// <summary>
        /// Schedule constructor
        /// </summary>
        public Schedule()
        {
            _logger = Logger.LoggingManager.GetLogger(nameof(Schedule));
        }

        /// <summary>
        /// Get shcedule text representation
        /// </summary>
        /// <returns></returns>
        public string GetScheduleDescription()
        {
            string description = string.Empty;
            if (Never)
            {
                description = "Never";
            }
            else if (Always)
            {
                description = "Always";
            }
            else
            {
                if (Start.HasValue)
                    description += $"From {Start.Value} ";
                if (End.HasValue)
                    description += $"To {End.Value}";
                if (DaysOfMonth?.Length > 0)
                {
                    string s = (DaysOfMonth.Length > 1) ? "s" : "";
                    description += $" At day{s} of month{s}: {string.Join(", ", DaysOfMonth)}";
                }
                if (DaysOfWeek?.Length > 0)
                {
                    string s = (DaysOfWeek.Length > 1) ? "s" : "";
                    description += $" At week day{s}: {string.Join(", ", DaysOfWeek)}";
                }
                if (Repeat.HasValue)
                    description += $" Repeat every: {Repeat.Value}";
            }

            return description;
        }

        /// <summary>
        /// Calculate next Schedule time window
        /// </summary>
        /// <param name="ignoreRepeat"></param>
        /// <param name="calculateFrom"></param>
        /// <returns>DateTime for next schedule time window</returns>
        public DateTime GetNext(bool ignoreRepeat = false, DateTime? calculateFrom = null)
        {
            _logger.Trace($"{nameof(GetNextRun)} {nameof(ignoreRepeat)}: {ignoreRepeat} {nameof(calculateFrom)} : {calculateFrom} {nameof(Schedule)}: {this}");

            if (Never)
            {
                _logger.Trace($"Result: Never = {DateTime.MaxValue}");
                return DateTime.MaxValue;
            }

            DateTime dt = (calculateFrom.HasValue)
                ? calculateFrom.Value
                : DateTime.Now;

            if (!ignoreRepeat && Repeat.HasValue)
                dt = dt.Add(Repeat.Value);

            if (!IsNow(dt))
            {
                if (DaysOfMonth?.Length > 0)
                {
                    var oldDt = dt;
                    dt = GetNextMonthDayDateTimeFromDaysArray(DaysOfMonth, dt);
                    _logger.Trace($"DaysOfMonth?.Length > 0: Old: {oldDt} New: {dt}");
                }

                if (DaysOfWeek?.Length > 0)
                {
                    var oldDt = dt;
                    dt = GetNextWeekDayDateTimeFromWeekArray(DaysOfWeek, dt);
                    _logger.Trace($"DaysOfWeek?.Length > 0: Old: {oldDt} New: {dt}");
                }

                if ((End.HasValue && Start.HasValue) && !IsTimeBetwean(Start.Value, End.Value, dt))
                {
                    var oldDt = dt;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, Start.Value.Hours, Start.Value.Minutes, Start.Value.Minutes);
                    _logger.Trace($"(End.HasValue && Start.HasValue) && !IsTimeBetwean(Start.Value, End.Value, dt): Old: {oldDt} New: {dt}");
                }
            }
            else
            {
                _logger.Trace($"CanRun = True");
            }

            _logger.Trace($"Result: {dt}");
            return dt;
        }

        /// <inheritdoc cref="GetNext(bool, DateTime?)" />
        /// <see cref="GetNext(bool, DateTime?) "/>
        [Obsolete]
        public DateTime GetNextRun(bool ignoreRepeat = false, DateTime? calculateFrom = null) => GetNext(ignoreRepeat, calculateFrom);

        /// <summary>
        /// Make calculation for schedule to check is currently is in configured time window
        /// </summary>
        /// <param name="calculateFrom">(Optional) DateTime for calculation. Default: DateTime.Now</param>
        /// <returns>True if schedule is in given time window</returns>
        public bool IsNow(DateTime? calculateFrom = null)
        {
            if (Never)
            {
                return false;
            }
            else if (Always)
            {
                return true;
            }

            DateTime dt = (calculateFrom.HasValue)
            ? calculateFrom.Value
            : DateTime.Now;

            if (DaysOfWeek?.Length > 0 && !(DaysOfWeek?.Contains(dt.DayOfWeek) ?? false))
            {
                return false;
            }
            if (DaysOfMonth?.Length > 0 && !(DaysOfMonth?.Contains(dt.Day) ?? false))
            {
                return false;
            }
            if ((Start.HasValue && End.HasValue) && !IsTimeBetwean(Start.Value, End.Value, dt))
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc cref="IsNow(DateTime?)"/>
        /// <see cref="IsNow(DateTime?)" />
        [Obsolete]
        public bool CanRun(DateTime? calculateFrom = null) => IsNow(calculateFrom);

        /// <summary>
        /// Get DateTime for next day for given weekday
        /// </summary>
        /// <param name="daysOfWeek"></param>
        /// <param name="fromDT"></param>
        /// <returns></returns>
        public static DateTime GetNextWeekDayDateTimeFromWeekArray(DayOfWeek[] daysOfWeek, DateTime fromDT)
        {
            foreach (var dtw in daysOfWeek.OrderByDescending(x => x))
            {
                return GetNextWeekday(fromDT, dtw);
            }

            return fromDT;
        }

        /// <summary>
        /// Get DateTime for next month day
        /// </summary>
        /// <param name="days"></param>
        /// <param name="fromDT"></param>
        /// <returns></returns>
        public static DateTime GetNextMonthDayDateTimeFromDaysArray(int[] days, DateTime fromDT)
        {
            var dates = days.Select(x => new DateTime(fromDT.Year, fromDT.Month, x)).OrderBy(o => o);
            var ddt = dates.FirstOrDefault(x => x >= fromDT);
            if (ddt != DateTime.MinValue) return ddt;
            else return new DateTime(fromDT.AddMonths(1).Year, fromDT.AddMonths(1).Month, dates.First().Day);
        }

        /// <summary>
        /// Get DateTime for next day for given weekday
        /// </summary>
        /// <param name="start"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        /// <summary>
        /// Check if given datetime is given TimeSpan range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        public static bool IsTimeBetwean(TimeSpan start, TimeSpan end, DateTime now)
        {
            if ((now.TimeOfDay >= start) && (now.TimeOfDay <= end)) return true;
            else return false;
        }

        /// <summary>
        /// Make schedule text representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ScheduleString.Parse(this);
        }

        /// <summary>
        /// Check is given object is a Schedule with same parameters
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Schedule a)
            {
                Schedule b = this;
                return string.Equals(a.ToString(), b.ToString(), StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Get Schedule hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hash = (Convert.ToInt32(Always) + Convert.ToInt32(Never));
            if (Start.HasValue)
                hash += Convert.ToInt32(Start.Value.TotalSeconds);
            if (End.HasValue)
                hash += Convert.ToInt32(End.Value.TotalSeconds);
            if (Repeat.HasValue)
                hash += Convert.ToInt32(Repeat.Value.TotalSeconds);
            if (DaysOfMonth?.Length > 0)
                hash += DaysOfMonth?.Sum() ?? 0;
            if (DaysOfWeek?.Length > 0)
                hash += DaysOfWeek?.Select(x => (int)x)?.Sum() ?? 0;

            return hash;
        }
    }
}
