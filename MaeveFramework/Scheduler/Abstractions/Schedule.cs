using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaeveFramework.Scheduler.Abstractions
{
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

        public Schedule()
        {
            _logger = Logger.LoggingManager.GetLogger(nameof(Schedule));
        }

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

        public DateTime GetNextRun(bool ignoreRepeat = false, DateTime? calculateFrom = null)
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

            if (!CanRun(dt))
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

                if (Start.HasValue && Repeat.GetValueOrDefault(TimeSpan.Zero) >= TimeSpan.FromDays(1))
                {
                    var oldDt = dt;
                    dt = new DateTime(dt.Year, dt.Month, dt.Day, Start.Value.Hours, Start.Value.Minutes, Start.Value.Seconds, Start.Value.Milliseconds);
                    _logger.Trace($"Start.HasValue && Repeat.GetValueOrDefault(TimeSpan.Zero) >= TimeSpan.FromDays(1): Old: {oldDt} New: {dt}");
                }

                if ((End.HasValue && Start.HasValue) && !IsTimeBetwean(Start.Value, End.Value, dt))
                {
                    var oldDt = dt;
                    dt = GetNextRun(true, new DateTime(dt.Year, dt.Month, dt.Day, Start.Value.Hours, Start.Value.Minutes, Start.Value.Seconds));
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

        public bool CanRun(DateTime? calculateFrom = null)
        {
            _logger.Trace($"{nameof(CanRun)} {nameof(calculateFrom)}: {calculateFrom} | {this}");

            if (Never)
            {
                _logger.Trace($"Result: False, because: {nameof(Never)} = {Never}");
                return false;
            }
            else if (Always)
            {
                _logger.Trace($"Result: False, because: {nameof(Always)} = {Always}");
                return true;
            }

            DateTime dt = (calculateFrom.HasValue)
            ? calculateFrom.Value
            : DateTime.Now;

            if (DaysOfWeek?.Length > 0 && !(DaysOfWeek?.Contains(dt.DayOfWeek) ?? false))
            {
                _logger.Trace($"Result: False, because: {nameof(DaysOfWeek)} dose not contains: {dt.DayOfWeek} ({dt})");
                return false;
            }
            if (DaysOfMonth?.Length > 0 && !(DaysOfMonth?.Contains(dt.Day) ?? false))
            {
                _logger.Trace($"Result: False, because: {nameof(DaysOfMonth)} dose not contains: {dt.Day} ({dt})");
                return false;
            }
            if ((Start.HasValue && End.HasValue) && !IsTimeBetwean(Start.Value, End.Value, dt))
            {
                _logger.Trace($"Result: False, because: {nameof(Start)} and {nameof(End)} not in range of {dt} ({dt})");
                return false;
            }

            _logger.Trace($"Result: True ({dt})");
            return true;
        }

        public static DateTime GetNextWeekDayDateTimeFromWeekArray(DayOfWeek[] daysOfWeek, DateTime fromDT)
        {
            foreach (var dtw in daysOfWeek.OrderByDescending(x => x))
            {
                return GetNextWeekday(fromDT, dtw);
            }

            return fromDT;
        }

        public static DateTime GetNextMonthDayDateTimeFromDaysArray(int[] days, DateTime fromDT)
        {
            var dates = days.Select(x => new DateTime(fromDT.Year, fromDT.Month, x)).OrderBy(o => o);
            var ddt = dates.FirstOrDefault(x => x >= fromDT);
            if (ddt != null) return ddt;
            else return new DateTime(fromDT.AddMonths(1).Year, fromDT.AddMonths(1).Month, dates.Last().Day);
        }

        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        public static bool IsTimeBetwean(TimeSpan start, TimeSpan end, DateTime now)
        {
            if ((now.TimeOfDay > start) && (now.TimeOfDay < end)) return true;
            else return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduleString">START_DT|END_DT|DayOfWeek(1,7)|DaysOfMonths(1,31)|Repeat(TS)|Never</param>
        /// <returns></returns>
        public override string ToString()
        {
            return ScheduleString.Parse(this);
        }

        public override bool Equals(object obj)
        {
            Schedule a = obj as Schedule;
            Schedule b = this;

            return string.Equals(a.ToString(), b.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

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
