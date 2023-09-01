using BackgroundJobs.Infrastructure.Services.Publishers;
using Quartz;

namespace BackgroundJobs.Infrastructure.Jobs;

public class LoggingJob : IJob
{
    private readonly IOutputResultPublisher _publisher;
    public LoggingJob(IOutputResultPublisher publisher)
    {
        _publisher = publisher;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        // Do something: log data, call a Lambda, HTTP request, etc.
        await Console.Out.WriteLineAsync($"Job {context.JobDetail.Key.Name} is being executed.");

        await _publisher.Publish(context);
    }
}