using Quartz;

namespace BackgroundJobs.Infrastructure.Services.Publishers;

public interface IOutputResultPublisher
{
    Task Publish(IJobExecutionContext context);
}