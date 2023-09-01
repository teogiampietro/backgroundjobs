using BackgroundJobs.Infrastructure.Interfaces;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace BackgroundJobs.Infrastructure.Services.Quartz;

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

    public async Task AddJobToScheduler(IJobRequestMessage jobRequestMessage, CancellationToken cancellationToken)
    {
        var jobDetail = CreateJobDetail(jobRequestMessage);
        var trigger = CreateTrigger(jobRequestMessage);

        if (Scheduler is not null)
            await Scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
    }

    private static IJobDetail CreateJobDetail(IJobRequestMessage jobRequestMessage)
    {
        return JobBuilder.Create(jobRequestMessage.Type)
            .WithIdentity(jobRequestMessage.Id.ToString(), jobRequestMessage.Type.Namespace!)
            .UsingJobData("ResultsTopic", jobRequestMessage.ResultsTopic)
            .Build();
    }

    private static ITrigger CreateTrigger(IJobRequestMessage jobRequestMessage)
    {
        var triggerBuilder = TriggerBuilder.Create()
            .WithIdentity($"{jobRequestMessage.Id.ToString()}.trigger", jobRequestMessage.Type.Namespace!)
            .WithPriority(jobRequestMessage.Priority);
        
        if (jobRequestMessage.CronExpression is null)
        {
            triggerBuilder = triggerBuilder
                .StartNow();
        }
        else
        {
            triggerBuilder = triggerBuilder
                .WithCronSchedule(jobRequestMessage.CronExpression)
                .WithDescription(jobRequestMessage.CronExpression);
        }
        
        return triggerBuilder.Build();
    }
}