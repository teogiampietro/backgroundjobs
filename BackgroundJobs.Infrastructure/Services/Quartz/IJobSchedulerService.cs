using BackgroundJobs.Infrastructure.Model;
using Quartz;

namespace BackgroundJobs.Infrastructure.Services.Quartz;

public interface IJobSchedulerService
{
    Type? GetJobType(string jobTypeName);
    Task AddJobToScheduler(JobRequest jobRequestMessage, CancellationToken cancellationToken);
    Task RerunJobFromScheduler(Guid jobId, CancellationToken cancellationToken);
    Task DeleteJobFromScheduler(Guid jobId, CancellationToken cancellationToken);
}