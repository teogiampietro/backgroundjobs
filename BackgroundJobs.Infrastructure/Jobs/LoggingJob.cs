using BackgroundJobs.Infrastructure.Services.Publishers;
using Quartz;

namespace BackgroundJobs.Infrastructure.Jobs;

public class LoggingJob : JobBase
{
    private readonly IResultsPublisherService _resultsPublisherService;

    public LoggingJob(IResultsPublisherService resultsPublisherService)
    {
        _resultsPublisherService = resultsPublisherService;
    }

    protected override async Task ExecuteJob(IJobExecutionContext context)
    {
        // This is a fast job, without delay.
        await Console.Out.WriteLineAsync($"Job {context.JobDetail.Key.Name} is being executed.");
        await Task.Delay(1000);
        await _resultsPublisherService.Publish(context);
        await Console.Out.WriteLineAsync($"Job {context.JobDetail.Key.Name} finished.");
    }
}