using Amazon.Lambda.Core;

namespace BackgroundJobs.LambdaExample.OutputProgress;

public interface IOutputProgressPublisher
{
    Task Publish(JobStatusMessage jobProgressMessage);
}