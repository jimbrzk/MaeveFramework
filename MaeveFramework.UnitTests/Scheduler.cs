using MaeveFramework.Scheduler;
using MaeveFramework.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using MaeveFramework.Scheduler.Abstractions;
using System.Threading.Tasks;

namespace MaeveFramework.UnitTests
{
    public class TestExceptionJob : JobBase<string>
    {
        public bool FirstRun { get; set; }

        public TestExceptionJob(MaeveFramework.Scheduler.Abstractions.Schedule schedule, string options) : base(schedule, options)
        {
            FirstRun = true;
        }

        public override void Job()
        {
            if (Options == "job")
                throw new Exception("Test exception");

            Logger.Debug("test - job");
            base.Job();
        }

        public override void OnStart()
        {
            if (Options == "start" && FirstRun)
            {
                FirstRun = false;
                throw new Exception("Test exception");
            }

            Logger.Debug("test - start");
            base.OnStart();
        }

        public override void OnStop()
        {
            if (Options == "stop")
                throw new Exception("Test exception");

            Logger.Debug("test - stop");
            base.OnStop();
        }
    }

    public class TestJob : JobBase<string>
    {
        public int runCount;

        public TestJob(MaeveFramework.Scheduler.Abstractions.Schedule schedule, string options) : base(schedule, options)
        {
            runCount = 0;
        }

        public override void Job()
        {
            Logger.Debug("test - job");
            runCount++;
            base.Job();
            Thread.Sleep(1000);
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
        }

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
            Assert.IsTrue(schedule.IsNow(), "Can't run schedule");
        }

        [TestMethod]
        public void ScheduleCase2()
        {
            Schedule schedule = new Schedule(repeat: 10.Seconds());
            Assert.AreEqual(ScheduleString.Parse("||||00:00:10|False"), schedule, "SS Parse failed");
            Assert.IsTrue(schedule.IsNow(), "Can't run schedule");
        }

        [TestMethod]
        public void ScheduleCase3()
        {
            Schedule schedule = new Schedule(repeat: 10.Seconds(), start: 8.Hours(), end: 10.Hours());
            Assert.AreEqual(ScheduleString.Parse("08:00:00|10:00:00|||00:00:10|False"), schedule, "SS Parse failed");
            Assert.IsFalse(schedule.IsNow(new DateTime(2020, 1, 1, 6, 0, 0)), "Can't run schedule"); // because scheduls is from 8 to 10 but we checking for 6
        }

        [TestMethod]
        public void ScheduleCase4()
        {
            Schedule schedule = new Schedule(start: 8.Hours(), end: 10.Hours(), daysOfWeek: new DayOfWeek[] { DayOfWeek.Monday }, daysOfMonth: new int[] { 1, 2, 3, 4, 5, 6, 7 });
            Assert.AreEqual(ScheduleString.Parse("08:00:00|10:00:00|Monday|1,2,3,4,5,6,7||False"), schedule, "SS Parse failed");
            DateTime testDate = new DateTime(2020, 1, 6, 7, 0, 0);
            Assert.IsFalse(schedule.IsNow(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 1);
            testDate = new DateTime(2020, 1, 6, 9, 0, 0);
            Assert.IsTrue(schedule.IsNow(testDate), "Can't run schedule at {0}, case {1}", testDate, 2);
            testDate = new DateTime(2020, 1, 6, 11, 0, 0);
            Assert.IsFalse(schedule.IsNow(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 3);
            testDate = new DateTime(2020, 1, 12, 9, 0, 0);
            Assert.IsFalse(schedule.IsNow(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 4);
            testDate = new DateTime(2020, 1, 12, 11, 0, 0);
            Assert.IsFalse(schedule.IsNow(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 5);
            testDate = new DateTime(2020, 1, 12, 6, 0, 0);
            Assert.IsFalse(schedule.IsNow(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 6);
            testDate = new DateTime(2020, 1, 5, 9, 0, 0);
            Assert.IsFalse(schedule.IsNow(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 7);
            testDate = new DateTime(2020, 1, 7, 6, 0, 0);
            Assert.IsFalse(schedule.IsNow(testDate), "Shcedule run at invalid time at {0}, case {1}", testDate, 8);
        }

        [TestMethod]
        public void ScheduleCase5()
        {
            DateTime testDate = new DateTime(2020, 1, 5, 7, 0, 0);

            Schedule schedule3 = ScheduleString.Parse("08:00:00|10:00:00|Monday||28.00:00:00|False");
            Assert.IsFalse(schedule3.IsNow(testDate), "Schedule too soon");

            var next3 = schedule3.GetNext(calculateFrom: testDate);
            Assert.IsTrue(schedule3.IsNow(next3), "Can't run at {0}", next3);

            Assert.IsFalse(schedule3.IsNow(next3.AddHours(4)), "Schedule too late");

            var next333 = schedule3.GetNext(calculateFrom: next3);
            Assert.IsTrue(schedule3.IsNow(next333), "Can't run at {0}", next333);

            var next33333 = schedule3.GetNext(calculateFrom: testDate.AddDays(1).AddSeconds(1));
            Assert.IsTrue(schedule3.IsNow(next33333), "Can't run at {0}", next33333);
        }

        [TestMethod]
        [Timeout(300000)]
        public void ScheduleCase6()
        {
            DateTime testDate = new DateTime(2020, 1, 6, 8, 0, 0);
            Schedule schedule = ScheduleString.Parse("08:00:00|10:00:00|Monday|1,2,3,4,5,6||False");

            Assert.IsTrue(schedule.IsNow(testDate), "Can't run schedule. Tested date: {0} Schedule: {1}", testDate, schedule);
            DateTime nextDate = schedule.GetNext(calculateFrom: testDate);
            Assert.IsTrue(schedule.IsNow(nextDate), "Can't run schedule. Tested date: {0} nextDate: {1} Schedule: {2}", testDate, nextDate, schedule);

            for (int d = 1; d < DateTime.DaysInMonth(testDate.Year, testDate.Month); d++)
            {
                DateTime nextTestDate = new DateTime(testDate.Year, testDate.Month, d, 0, 0, 0);

                for (int h = 23; h >= 0; h--)
                {
                    DateTime testDateFor = nextTestDate.AddHours(h);

                    if (Schedule.IsTimeBetwean(schedule.Start.Value, schedule.End.Value, testDateFor) && testDateFor.DayOfWeek == DayOfWeek.Monday && schedule.DaysOfMonth.Contains(testDateFor.Day))
                        Assert.IsTrue(schedule.IsNow(testDateFor), "Can't run schedule. Tested date: {0} Schedule: {1}", testDateFor, schedule);
                    else
                        Assert.IsFalse(schedule.IsNow(testDateFor), "Invalid date for run. Tested date: {0} Schedule: {1}", testDateFor, schedule);
                }
            }
        }

        [TestMethod]
        [Timeout(300000)]
        public void ScheduleCase7()
        {
            DateTime testDate = new DateTime(2020, 1, 6, 8, 0, 0);
            Schedule schedule = ScheduleString.Parse("08:00:00|10:00:00|Monday|||False");

            Assert.IsTrue(schedule.IsNow(testDate), "Can't run schedule. Tested date: {0} Schedule: {1}", testDate, schedule);
            DateTime nextDate = schedule.GetNext(calculateFrom: testDate);
            Assert.IsTrue(schedule.IsNow(nextDate), "Can't run schedule. Tested date: {0} nextDate: {1} Schedule: {2}", testDate, nextDate, schedule);

            for (int d = 1; d < DateTime.DaysInMonth(testDate.Year, testDate.Month); d++)
            {
                DateTime nextTestDate = new DateTime(testDate.Year, testDate.Month, d, 0, 0, 0);

                for (int h = 23; h >= 0; h--)
                {
                    DateTime testDateFor = nextTestDate.AddHours(h);

                    if (Schedule.IsTimeBetwean(schedule.Start.Value, schedule.End.Value, testDateFor) && testDateFor.DayOfWeek == DayOfWeek.Monday)
                        Assert.IsTrue(schedule.IsNow(testDateFor), "Can't run schedule. Tested date: {0} Schedule: {1} Start in range: {2} End in range: {3}", testDateFor, schedule, (schedule.Start >= 8.Hours()), (schedule.End <= 10.Hours()));
                    else
                        Assert.IsFalse(schedule.IsNow(testDateFor), "Invalid date for run. Tested date: {0} Schedule: {1} Start in range: {2} End in range: {3}", testDateFor, schedule, (schedule.Start >= 8.Hours()), (schedule.End <= 10.Hours()));
                }
            }
        }

        [TestMethod]
        [Timeout(300000)]
        public void ScheduleCase8()
        {
            DateTime testDate = new DateTime(2020, 1, 6, 8, 0, 0);
            Schedule schedule = ScheduleString.Parse("08:00:00|10:00:00||||False");

            Assert.IsTrue(schedule.IsNow(testDate), "Can't run schedule. Tested date: {0} Schedule: {1}", testDate, schedule);
            DateTime nextDate = schedule.GetNext(calculateFrom: testDate);
            Assert.IsTrue(schedule.IsNow(nextDate), "Can't run schedule. Tested date: {0} nextDate: {1} Schedule: {2}", testDate, nextDate, schedule);

            for (int d = 1; d < DateTime.DaysInMonth(testDate.Year, testDate.Month); d++)
            {
                DateTime nextTestDate = new DateTime(testDate.Year, testDate.Month, d, 0, 0, 0);

                for (int h = 23; h >= 0; h--)
                {
                    DateTime testDateFor = nextTestDate.AddHours(h);

                    if ((schedule.Start >= 8.Hours() && schedule.End <= 10.Hours()))
                        Assert.IsTrue(schedule.IsNow(testDateFor), "Can't run schedule. Tested date: {0} Schedule: {1}", testDateFor, schedule);
                    else
                        Assert.IsFalse(schedule.IsNow(testDateFor), "Invalid date for run. Tested date: {0} Schedule: {1}", testDateFor, schedule);
                }
            }
        }

        [TestMethod]
        public void ScheduleCase9()
        {
            DateTime testDate = new DateTime(2020, 1, 6, 8, 0, 0);
            Schedule schedule = ScheduleString.Parse("08:00:00|10:00:00|Monday|1,2,3,4,5,6|26.00:00:00|False");

            DateTime nextDate = schedule.GetNext(calculateFrom: testDate);

            Assert.IsTrue(nextDate.Subtract(testDate) >= new TimeSpan(0, 0, 0),
                "Calculation for next date is in minus position. Calculated date: {0} Seconds to next run: {1} Calculated from date: {2} Scheudle: {3}",
                nextDate, nextDate.Subtract(testDate).TotalSeconds, testDate, schedule);
            Assert.IsTrue(nextDate.DayOfWeek == DayOfWeek.Monday, "Calculated next date is not Monday! Next date: {0} Scedule: {1}", nextDate, schedule);
            Assert.IsTrue((nextDate.Hour >= 8 && nextDate.Hour <= 10), "Next date is not in expected hours range! Next run: {0} Schedule: {1}", nextDate, schedule);
            Assert.IsTrue(nextDate.Month == 2, "Next date is not next month! Next date: {0} Schedule: {1}", nextDate, schedule);
            Assert.IsTrue(schedule.IsNow(nextDate), "Can't run schedule! Next date {0} Schedule: {1}", nextDate, schedule);
        }

        [TestMethod]
        public void TestAddRemoveJobByName()
        {
            SchedulerManager.RemoveJob(nameof(TestJob));
            Guid jobguid = SchedulerManager.CreateJob(new TestJob(ScheduleString.Parse("||||00:00:05|"), "testowa opcja"));
            SchedulerManager.RemoveJob(nameof(TestJob));
            Assert.IsTrue(SchedulerManager.Job<TestJob>() == null, "Job not removed!");
        }

        [TestMethod]
        public void TestAddRemoveJobByGuid()
        {
            SchedulerManager.RemoveJob(nameof(TestJob));
            Guid jobguid = SchedulerManager.CreateJob(new TestJob(ScheduleString.Parse("||||00:00:05|"), "testowa opcja"));
            SchedulerManager.RemoveJob(jobguid);
            Assert.IsTrue(SchedulerManager.Job<TestJob>() == null, "Job not removed!");
        }

        [TestMethod]
        [Timeout(6000)]
        public void JobCase1()
        {
            SchedulerManager.RemoveJob(nameof(TestJob));
            Guid jobguid = SchedulerManager.CreateJob(new TestJob(ScheduleString.Parse("||||00:00:05|"), "testowa opcja"));
            Assert.IsTrue(SchedulerManager.Job<TestJob>() != null, "Failed to create job");


            SchedulerManager.StartAllJobs();
            Thread.Sleep(2000);
            DateTime dt = DateTime.Now;
            Assert.IsTrue((SchedulerManager.Job<TestJob>().State == JobStateEnum.Idle), "Job dose not have idle state ({0})", SchedulerManager.Job<TestJob>().State);
            Assert.IsTrue((SchedulerManager.Job<TestJob>().NextRun.Subtract(dt) > 1.Seconds()), "NextRun to soon! {0}", SchedulerManager.Job<TestJob>().NextRun.Subtract(dt));
        }

        [TestMethod]
        [Timeout(6000)]
        public void JobCase2()
        {
            SchedulerManager.RemoveJob(nameof(TestJob));
            Guid jobguid = SchedulerManager.CreateJob(new TestJob(ScheduleString.Parse("||||00:00:05|"), "testowa opcja"));
            Assert.IsTrue(SchedulerManager.Job<TestJob>() != null, "Failed to create job");

            SchedulerManager.StartAllJobs();
            Thread.Sleep(2000);
            DateTime dt = DateTime.Now;
            Assert.IsTrue((SchedulerManager.Job<TestJob>().State == MaeveFramework.Scheduler.Abstractions.JobStateEnum.Idle), "Job dose not have idle state ({0})", SchedulerManager.Job<TestJob>().State);
            Assert.IsTrue((SchedulerManager.Job<TestJob>().NextRun.Subtract(dt) > 1.Seconds()), "NextRun to soon! {0}", SchedulerManager.Job<TestJob>().NextRun.Subtract(dt));
        }

        [TestMethod]
        [Timeout(13000)]
        public void JobCase3()
        {
            SchedulerManager.RemoveJob(nameof(TestExceptionJob));
            Guid jobguid = SchedulerManager.CreateJob(new TestExceptionJob(ScheduleString.Parse("||||00:00:05|"), "start"));
            Assert.IsTrue(SchedulerManager.Job<TestExceptionJob>() != null, "Failed to create job");

            SchedulerManager.StartAllJobs();

            Assert.IsTrue(SchedulerManager.JobController<TestExceptionJob>().WaitForState(JobStateEnum.Crash, 2.Seconds()), "Job is not in crash state. Current state: {0}", SchedulerManager.Job<TestExceptionJob>().State);

            Assert.IsTrue(SchedulerManager.JobController<TestExceptionJob>().WaitForState(new[] { JobStateEnum.Working, JobStateEnum.Idle }, 12.Seconds()), "Job is not in valid state. Current state: {0}", SchedulerManager.Job<TestExceptionJob>().State);
        }

        [TestMethod]
        [Timeout(12000)]
        public void WakeJob()
        {
            SchedulerManager.RemoveJob(nameof(TestExceptionJob));

            Guid jobguid = SchedulerManager.CreateJob(new TestExceptionJob(ScheduleString.Parse($"||{DateTime.Now.AddDays(1).DayOfWeek}|||"), string.Empty));
            Assert.IsTrue(SchedulerManager.Job<TestExceptionJob>() != null, "Failed to create job");

            SchedulerManager.StartAllJobs();
            Assert.IsTrue(SchedulerManager.JobController<TestExceptionJob>().WaitForState(JobStateEnum.Idle, 5.Seconds()), "Job failed to get into Idle state in 5 seconds");

            SchedulerManager.JobController<TestExceptionJob>().Wake();

            Thread.Sleep(3000);

            Assert.IsTrue(SchedulerManager.Job<TestExceptionJob>().LastRun.HasValue, "Job not executed after wakeup call", SchedulerManager.Job<TestExceptionJob>().LastRun?.ToString());
        }

        [TestMethod]
        public void TestConcurency()
        {
            SchedulerManager.RemoveJob(nameof(TestJob));
            Guid jobguid = SchedulerManager.CreateJob(new TestJob(ScheduleString.Parse("||||00:00:05|"), "start"));
            Assert.IsTrue(SchedulerManager.Job<TestJob>() != null, "Failed to create job");

            SchedulerManager.StartAllJobs();
            SchedulerManager.JobController<TestJob>().WaitForState(JobStateEnum.Idle, 5.Seconds());
            Assert.AreEqual(1, SchedulerManager.Job<TestJob>().runCount, "Job has not expected run count");

            SchedulerManager.JobController<TestJob>().Wake();
            SchedulerManager.JobController<TestJob>().WaitForState(JobStateEnum.Idle, 5.Seconds());
            Assert.AreEqual(2, SchedulerManager.Job<TestJob>().runCount, "Job has not expected run count");

            Task.Run(() => SchedulerManager.JobController<TestJob>().Wake());
            Task.Run(() => SchedulerManager.JobController<TestJob>().Wake());
            Thread.Sleep(300);
            SchedulerManager.JobController<TestJob>().Wake();
            SchedulerManager.JobController<TestJob>().WaitForState(new[] { JobStateEnum.Wake, JobStateEnum.Working }, 5.Seconds());
            SchedulerManager.JobController<TestJob>().WaitForState(JobStateEnum.Idle, 5.Seconds());
            Assert.AreEqual(3, SchedulerManager.Job<TestJob>().runCount, "Job has not expected run count");
        }

        [TestMethod]
        public void TestScheduleWithoutEndTime()
        {
            var schedule = new Schedule(DateTime.Now.TimeOfDay, null, new[] { DateTime.Now.DayOfWeek }, null, 28.Days());
            var next= schedule.GetNext();
            
        }
    }
}
