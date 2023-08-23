using BackgroundJobs.Infrastructure.Model;
using Quartz;

namespace BackgroundJobs.Infrastructure.Service;

public class JobExecute : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        JobLogger.Log(context);
        return Task.CompletedTask;
    }
}