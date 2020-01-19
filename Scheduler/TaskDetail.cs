using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MaeveFramework.Scheduler.JobObject
{
    public enum JobState
    {
        NotSet = 0,
        Idle = 1, 
        Stopped = 2,
        Stopping = 3,
        Started = 4,
        Starting = 5,
        NotStarted = 6,
        Working = 7,
        Crash = 8
    }

    public class Schedule
    {
        /// <summary>
        /// Start of job date time
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// Job end date time
        /// </summary>
        public DateTime? End { get; set; }
        /// <summary>
        /// Days of week when job can be run. Every day when null
        /// </summary>
        public DayOfWeek[] DaysOfWeek { get; set; }
        /// <summary>
        /// Repeat timespan. One time job when nuln
        /// </summary>
        public TimeSpan? Repeat { get; set; }
        /// <summary>
        /// Is this one time job
        /// </summary>
        public bool OneTime { get => (Repeat.HasValue) ? false : false; }

        public Schedule(DateTime? start = null, DateTime? end = null, DayOfWeek[] daysOfWeek = null, TimeSpan? repeat = null)
        {
            Start = start;
            End = end;
            DaysOfWeek = daysOfWeek;
            Repeat = repeat;
        }

        public DateTime GetNextRun(bool ignoreRepeat = false)
        {
            DateTime dt = DateTime.Now;

            if(!ignoreRepeat && Repeat.HasValue)
                dt = dt.Add(Repeat.Value);

            if (DaysOfWeek != null)
                dt = GetNextWeekDayDateTimeFromWeekArray(DaysOfWeek, dt);

            if (Start.HasValue && Repeat.GetValueOrDefault(TimeSpan.Zero) >= TimeSpan.FromDays(1)) 
                dt = new DateTime(dt.Year, dt.Month, dt.Day, Start.Value.Hour, Start.Value.Minute, Start.Value.Second, Start.Value.Millisecond);

            return dt;
        }

        public DateTime GetNextWeekDayDateTimeFromWeekArray(DayOfWeek[] daysOfWeek, DateTime fromDT)
        {
            foreach (var dtw in daysOfWeek.OrderByDescending(x => x))
            {
                return GetNextWeekday(fromDT, dtw);
            }

            return fromDT;
        }

        public DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
    }

    public class JobDetails
    {
        public string Name { get; set; }
        public Type JobType { get; set; }
        public Task SystemTask { get; set; }
        public CancellationTokenSource JobCancelToken { get; set; }
        public Schedule Schedule { get; set; }
        public JobState? State { get; set; }
        public string StateName { get => Enum.GetName(typeof(JobState), State.GetValueOrDefault(0)); }
        public DateTime? NextRun { get; set; }
        public DateTime? LastRun { get; set; }
        public Guid Guid { get; set; }
    }
}
;