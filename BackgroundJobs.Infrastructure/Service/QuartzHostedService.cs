using BackgroundJobs.Infrastructure.Model;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace BackgroundJobs.Infrastructure.Service;

public class QuartzHostedService : IHostedService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;
    private readonly IEnumerable<MyJob> _myJobs;

    public QuartzHostedService(
        ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        IEnumerable<MyJob> myJobs)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
        _myJobs = myJobs;
    }

    private IScheduler Scheduler { get; set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        Scheduler.JobFactory = _jobFactory;
        foreach (var myJob in _myJobs)
        {
            var job = CreateJob(myJob);
            var trigger = CreateTrigger(myJob);
            await Scheduler.ScheduleJob(job, trigger, cancellationToken);
        }

        await Scheduler.Start(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Scheduler?.Shutdown(cancellationToken);
    }

    private static IJobDetail CreateJob(MyJob myJob)
    {
        return JobBuilder.Create(myJob.Type)
            .WithIdentity(myJob.Type.FullName, myJob.Type.Namespace)
            .WithDescription(myJob.Resource)
            .Build();
    }

    private static ITrigger CreateTrigger(MyJob myJob)
    {
        return TriggerBuilder.Create()
            .WithIdentity($"{myJob.Type.FullName}.trigger", myJob.Type.Namespace)
            .WithDescription(myJob.CronExpression)
            .WithCronSchedule(myJob.CronExpression)
            .WithPriority(myJob.Priority)
            .Build();
    }
}