using BackgroundJobs.Service.Publisher;
using Quartz;

namespace BackgroundJobs.Service.Quartz;

public class LoggingJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        // Do something: log data, call a Lambda, HTTP request, etc.
        await Console.Out.WriteLineAsync($"Job {context.JobDetail.Key.Name} is being executed.");

        await SnsResultsPublisherService.Publish(context);
    }
}