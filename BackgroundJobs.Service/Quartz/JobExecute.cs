using BackgroundJobs.Service.Model;
using BackgroundJobs.Service.Publisher;
using Quartz;

namespace BackgroundJobs.Service.Quartz;

public class JobExecute : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        // Do something: log data, call a Lambda, HTTP request, etc.
        JobLogger.Log(context);

        await SnsResultsPublisherService.Publish(context);
    }
}