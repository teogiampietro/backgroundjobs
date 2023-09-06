using BackgroundJobs.Infrastructure.Services.Publishers;
using Quartz;

namespace BackgroundJobs.Infrastructure.Jobs;

public class SlowLoggingJob : IJob
{
    private readonly IOutputResultPublisher _publisher;

    public SlowLoggingJob(IOutputResultPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        // This is a slow job, with 15 seconds of delay.
        await Console.Out.WriteLineAsync($"Job {context.JobDetail.Key.Name} is being executed.");
        await Task.Delay(15000);
        await _publisher.Publish(context);
        await Console.Out.WriteLineAsync($"Job {context.JobDetail.Key.Name} finished.");
    }
}