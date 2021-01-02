# MaeveFramework
### Just another usefull .NET library... :) <3

![CodeQL](https://github.com/kubala156/MaeveFramework/workflows/CodeQL/badge.svg)
![Nuget](https://img.shields.io/nuget/v/MaeveFramework)

## + Jobs - Simple task scheduling
### Create a Job

```
    public class TestJob : JobBase<string>
    {
        public TestJob(MaeveFramework.Scheduler.Abstractions.Schedule schedule, string options) : base(schedule, options)
        {
			
        }

        public override void Job()
        {
            Logger.Debug("Runned by schedule");
        }

        public override void OnStart()
        {
            Logger.Debug("On job start, once");
        }

        public override void OnStop()
        {
            Logger.Debug("On Job stop, once");
        }
    }
```

### Add job to scheduler

```
// Create job from class TestJob with schedule, run every 5 minutes betwean 10 and 8 at Monday whean day of month is 1, 2 or 3.

MaeveFramework.Scheduler.SchedulerManager.CreateJob(new TestJob(new Schedule(start: TimeSpan.FromHours(10), end: TimeSpan.FromHours(20), daysOfWeek: new DayOfWeek[] { DayOfWeek.Monday }, daysOfMonth: new int[] { 1, 2, 3 }, repeat: TimeSpan.FromMinutes(5)), "test option"));
```

### And start

```
MaeveFramework.Scheduler.SchedulerManager.StartAllJobs();
```

### You can access the Job by calling it's type or GUID

```
TestJob job = MaeveFramework.Scheduler.SchedulerManager.Job<TestJob>();
// Or
job = TestJob job = MaeveFramework.Scheduler.SchedulerManager.Job<TestJob>(job.Guid);
var status = job.Status;
DateTime? lastRun = job.LastRun;
```

### Can't wait for next job run from schedule? Force it!

```
SchedulerManager.JobController<TestJob>().Wake();
```

This wake up call will execute the job ignoring the schedule. Be aware that new NextRun time will be calculated after that.

## + Helpers

### - DynamicLinq
### - AbstrationsHelpers
```
public void Knock()
{
	string whoss_there = MaeveFramework.Helpers.AbstrationsHelpers.NameOfCallingClass();
}
```
### - ArgsArrayConverter
```
/// string[] { "-test arg" }
public static Main(string[] args)
{
	// { { "test", "arg" } }
	Dictionary<string, string> dictArgs = MaeveFramework.Helpers.ArgsArrayConverter.ArgsToDictionary(args, "-");
}
```
### - IntToTimespan
```
using MaeveFramework.Helpers;

public TimeSpan Get10SecondsTS() 
{
	return 10.Seconds();
}
```
### - PropertiesResolver
```
string text = "Current date and time: {{now}}";
string result = MaeveFramework.Helpers.Resolve(text, new { now = DateTime.Now }, false, Consts.Helpers.TAG_OPEN, Consts.Helpers.TAG_CLOSE, Consts.Helpers.ARRAY_SEPARATOR);
```
### - Retry

```
MaeveFramework.Helpers.Retry.Do(() =>
{
	Console.WriteLine("I just trying to do somting here. If i throw exception i will try again 2 time with 10 seconds pause!");
    Console.WriteLine("And if i keep throwing exceptions, it will finaly throw agregated exception.");
}, TimeSpan.FromSeconds(10), 2);
```

=======
