using BackgroundJobs.Infrastructure.Messages;
using Quartz;

namespace BackgroundJobs.Infrastructure.Services.Publishers;

public interface IResultsPublisherService
{
    Task Publish(IJobExecutionContext context);
    Task Publish(JobResultMessage jobResultMessage);
}