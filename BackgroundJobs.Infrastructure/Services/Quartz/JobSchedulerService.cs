using BackgroundJobs.Infrastructure.Model;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace BackgroundJobs.Infrastructure.Services.Quartz;

public class JobSchedulerService : BackgroundService, IJobSchedulerService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;

    public JobSchedulerService(ISchedulerFactory schedulerFactory, IJobFactory jobFactory)
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

    public async Task AddJobToScheduler(JobRequest jobRequestMessage, CancellationToken cancellationToken)
    {
        var jobDetail = CreateJobDetail(jobRequestMessage);
        var trigger = CreateTrigger(jobRequestMessage);

        if (Scheduler is not null)
            await Scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
    }

    private static IJobDetail CreateJobDetail(JobRequest jobRequestMessage)
    {
        return JobBuilder.Create(jobRequestMessage.JobType)
            .WithIdentity(jobRequestMessage.JobId.ToString(), jobRequestMessage.JobType.Namespace!)
            .UsingJobData("ResultsTopic", jobRequestMessage.ResultsTopicName)
            .Build();
    }

    private static ITrigger CreateTrigger(JobRequest jobRequestMessage)
    {
        var triggerBuilder = TriggerBuilder.Create()
            .WithIdentity($"{jobRequestMessage.JobId.ToString()}.trigger", jobRequestMessage.JobType.Namespace!)
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