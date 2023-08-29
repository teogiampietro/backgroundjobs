using BackgroundJobs.Service.Model;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace BackgroundJobs.Service.Quartz;

public class QuartzHostedService : BackgroundService, IQuartzService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;

    public QuartzHostedService(
        ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
    }
    
    private IScheduler? Scheduler { get; set; }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        Scheduler.JobFactory = _jobFactory;
        
        await Scheduler.Start(cancellationToken);
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (Scheduler is not null)
            await Scheduler.Shutdown(cancellationToken);
    }

    public async Task AddJobToScheduler(MyJob myJob, CancellationToken cancellationToken)
    {
        var job = CreateJob(myJob);
        var trigger = CreateTrigger(myJob);
        
        if (Scheduler is not null)
            await Scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    private static IJobDetail CreateJob(MyJob myJob)
    {
        return JobBuilder.Create(myJob.Type)
            .WithIdentity("name", "group")
            // .WithDescription(myJob.Resource)
            .Build();
    }

    private static ITrigger CreateTrigger(MyJob myJob)
    {
        return TriggerBuilder.Create()
            .WithIdentity("triggerName", "group")
            // .WithDescription(myJob.CronExpression)
            // .WithCronSchedule(myJob.CronExpression)
            // .WithPriority(myJob.Priority)
            .StartNow()
            .Build();
    }
}