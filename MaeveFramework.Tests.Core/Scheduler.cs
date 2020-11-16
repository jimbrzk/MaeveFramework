using MaeveFramework.Scheduler;
using MaeveFramework.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using MaeveFramework.Scheduler.Abstractions;

namespace MaeveFramework.Tests.Core
{
    public class TestJob : JobBase<string>
    {
        public TestJob(MaeveFramework.Scheduler.Abstractions.Schedule schedule, string options) : base(schedule, options)
        {

        }

        public override void Job()
        {
            Logger.Debug("test - job");
            base.Job();
        }

        public override void OnStart()
        {
            Logger.Debug("test - start");
            base.OnStart();
        }

        public override void OnStop()
        {
            Logger.Debug("test - stop");
            base.OnStop();
        }
    }

    [TestClass]
    public class Scheduler
    {
        public Scheduler()
        {
            MaeveFramework.Logger.LoggingManager.UseDebug();
            var scheduler = new MaeveFramework.Scheduler.SchedulerManager();
        }

        //[TestMethod]
        //public void ScheduleParseRepeats()
        //{
        //    DateTime dtToday = DateTime.Today;

        //    var schedule = ScheduleString.Parse($"00:00:00||||00:00:1|");
        //    for (int d = 1; d < DateTime.DaysInMonth(dtToday.Year, dtToday.Month); d++)
        //    {
        //        for (int h = 23; h >= 0; h--)
        //        {
        //            for (int m = 59; m >= 0; m--)
        //            {
        //                for (int s = 59; s >= 0; s--)
        //                {
        //                    var dt = schedule.GetNextRun(false, new DateTime(dtToday.Year, dtToday.Month, d, h, m, s));
        //                    var expected = new DateTime(dtToday.Year, dtToday.Month, d, h, m, s).AddSeconds(1);
        //                    Assert.AreEqual(expected, dt, $"Invalid next run: {dt} != {expected} ({schedule} / {d} {h} {m} {s})");
        //                }
        //            }
        //        }
        //    }
        //}

        [TestMethod]
        public void ScheduleGetDescription()
        {
            Schedule schedule = new Schedule(repeat: 10.Seconds());
            var text = schedule.GetScheduleDescription();
            Assert.IsFalse(String.IsNullOrWhiteSpace(text), "Failed to get description for schedule", text);
        }

        [TestMethod]
        public void ScheduleCase1()
        {
            Schedule schedule = new Schedule(repeat: 10.Seconds());
            Assert.AreEqual("||||00:00:10|False", ScheduleString.Parse(schedule), "SS Parse failed");
            Assert.IsTrue(schedule.CanRun(), "Can't run schedule");
        }

        [TestMethod]
        public void ScheduleCase2()
        {
            Schedule schedule = new Schedule(repeat: 10.Seconds());
            Assert.AreEqual(ScheduleString.Parse("||||00:00:10|False"), schedule, "SS Parse failed");
            Assert.IsTrue(schedule.CanRun(), "Can't run schedule");
        }

        [TestMethod]
        public void ScheduleCase3()
        {
            Schedule schedule = new Schedule(repeat: 10.Seconds(), start: 8.Hours(), end: 10.Hours());
            Assert.AreEqual(ScheduleString.Parse("08:00:00|10:00:00|||00:00:10|False"), schedule, "SS Parse failed");
            Assert.IsFalse(schedule.CanRun(new DateTime(2020, 1, 1, 6, 0, 0)), "Can't run schedule"); // because scheduls is from 8 to 10 but we checking for 6
        }

        [TestMethod]
        public void ScheduleCase4()
        {
            Schedule schedule = new Schedule(start: 8.Hours(), end: 10.Hours(), daysOfWeek: new DayOfWeek[] { DayOfWeek.Monday }, daysOfMonth: new int[] { 1, 2, 3, 4, 5, 6, 7 });
            Assert.AreEqual(ScheduleString.Parse("08:00:00|10:00:00|Monday|1,2,3,4,5,6,7||False"), schedule, "SS Parse failed");
            DateTime testDate = new DateTime(2020, 1, 6, 7, 0, 0);
            Assert.IsFalse(schedule.CanRun(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 1);
            testDate = new DateTime(2020, 1, 6, 9, 0, 0);
            Assert.IsTrue(schedule.CanRun(testDate), "Can't run schedule at {0}, case {1}", testDate, 2);
            testDate = new DateTime(2020, 1, 6, 11, 0, 0);
            Assert.IsFalse(schedule.CanRun(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 3);
            testDate = new DateTime(2020, 1, 12, 9, 0, 0);
            Assert.IsFalse(schedule.CanRun(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 4);
            testDate = new DateTime(2020, 1, 12, 11, 0, 0);
            Assert.IsFalse(schedule.CanRun(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 5);
            testDate = new DateTime(2020, 1, 12, 6, 0, 0);
            Assert.IsFalse(schedule.CanRun(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 6);
            testDate = new DateTime(2020, 1, 5, 9, 0, 0);
            Assert.IsFalse(schedule.CanRun(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 7);
            testDate = new DateTime(2020, 1, 7, 6, 0, 0);
            Assert.IsFalse(schedule.CanRun(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 8);
        }

        [TestMethod]
        public void ScheduleCase5()
        {
            DateTime testDate = new DateTime(2020, 1, 5, 7, 0, 0);

            Schedule schedule3 = ScheduleString.Parse("08:00:00|10:00:00|Monday||28.00:00:00|False");
            Assert.IsFalse(schedule3.CanRun(testDate), "Schedule too soon");

            var next3 = schedule3.GetNextRun(calculateFrom: testDate);
            Assert.IsTrue(schedule3.CanRun(next3), "Can't run at {0}", next3);

            Assert.IsFalse(schedule3.CanRun(next3.AddHours(4)), "Schedule too late");

            var next333 = schedule3.GetNextRun(calculateFrom: next3);
            Assert.IsTrue(schedule3.CanRun(next333), "Can't run at {0}", next333);

            var next33333 = schedule3.GetNextRun(calculateFrom: testDate.AddDays(1).AddSeconds(1));
            Assert.IsTrue(schedule3.CanRun(next33333), "Can't run at {0}", next33333);
        }

        [TestMethod]
        public void JobCase1()
        {
            Guid jobguid = SchedulerManager.CreateJob(new TestJob(ScheduleString.Parse("||||00:00:05|"), "testowa opcja"));
            Assert.IsTrue(SchedulerManager.Job<TestJob>() != null, "Failed to create job");

            SchedulerManager.StartAllJobs();
            Thread.Sleep(4000);
            Assert.IsTrue((SchedulerManager.Job<TestJob>().State == MaeveFramework.Scheduler.Abstractions.JobStateEnum.Idle), $"Job dose not have idle state ({SchedulerManager.Job<TestJob>().State})");
        }
    }
}
