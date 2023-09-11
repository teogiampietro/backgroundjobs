using BackgroundJobs.Infrastructure.Model;

namespace BackgroundJobs.Infrastructure.Services.Quartz;

public interface IJobSchedulerService
{
    Type? GetJobType(string jobTypeName);
    Task AddJobToScheduler(JobRequest jobRequestMessage, CancellationToken cancellationToken);
}