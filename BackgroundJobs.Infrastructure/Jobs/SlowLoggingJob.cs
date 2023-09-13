using BackgroundJobs.Infrastructure.Services.Publishers;
using Quartz;

namespace BackgroundJobs.Infrastructure.Jobs;

public class SlowLoggingJob : JobBase
{
    private readonly IResultsPublisherService _resultsPublisherService;

    public SlowLoggingJob(IResultsPublisherService resultsPublisherService)
    {
        _resultsPublisherService = resultsPublisherService;
    }

    protected override async Task ExecuteJob(IJobExecutionContext context)
    {
        // This is a slow job, with 15 seconds of delay.
        await Console.Out.WriteLineAsync($"Job {context.JobDetail.Key.Name} is being executed.");
        await Task.Delay(15000);
        await _resultsPublisherService.Publish(context);
        await Console.Out.WriteLineAsync($"Job {context.JobDetail.Key.Name} finished.");
    }

}