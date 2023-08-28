using BackgroundJobs.Service.Model;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace BackgroundJobs.Service.Quartz;

public class QuartzHostedService : BackgroundService, IQuartzService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;
    private IScheduler Scheduler { get; set; }

    public QuartzHostedService(
        ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        Scheduler.JobFactory = _jobFactory;
        await Scheduler.Start(cancellationToken);
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Scheduler?.Shutdown(cancellationToken);
    }

    public async Task AddJobToScheduler(CancellationToken cancellationToken, MyJob myJob)
    {
        var job = CreateJob(myJob);
        var trigger = CreateTrigger(myJob);
        JobLogger.LogMyJob(myJob);
        await Scheduler.ScheduleJob(job, trigger, cancellationToken);
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