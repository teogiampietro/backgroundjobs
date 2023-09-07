using BackgroundJobs.Infrastructure.Model;

namespace BackgroundJobs.Infrastructure.Services.Quartz;

public interface IJobSchedulerService
{
    Task AddJobToScheduler(JobRequest jobRequestMessage, CancellationToken cancellationToken);
}