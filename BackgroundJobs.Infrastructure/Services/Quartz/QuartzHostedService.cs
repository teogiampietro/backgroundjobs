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

    public async Task AddJobToScheduler(IJobRequest jobRequest, CancellationToken cancellationToken)
    {
        var jobDetail = CreateJobDetail(jobRequest);
        var trigger = CreateTrigger(jobRequest);

        if (Scheduler is not null)
            await Scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
    }

    private static IJobDetail CreateJobDetail(IJobRequest jobRequest)
    {
        return JobBuilder.Create(jobRequest.Type)
            .WithIdentity(jobRequest.Id.ToString(), jobRequest.Type.Namespace!)
            .UsingJobData("ResultsTopic", jobRequest.ResultsTopic)
            .Build();
    }

    private static ITrigger CreateTrigger(IJobRequest jobRequest)
    {
        var triggerBuilder = TriggerBuilder.Create()
            .WithIdentity($"{jobRequest.Id.ToString()}.trigger", jobRequest.Type.Namespace!)
            .WithPriority(jobRequest.Priority);
        
        if (jobRequest.CronExpression is null)
        {
            triggerBuilder = triggerBuilder
                .StartNow();
        }
        else
        {
            triggerBuilder = triggerBuilder
                .WithCronSchedule(jobRequest.CronExpression)
                .WithDescription(jobRequest.CronExpression);
        }
        
        return triggerBuilder.Build();
    }
}