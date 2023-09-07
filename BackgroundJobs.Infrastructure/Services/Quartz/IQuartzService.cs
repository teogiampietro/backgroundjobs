using BackgroundJobs.Infrastructure.Model;

namespace BackgroundJobs.Infrastructure.Services.Quartz;

public interface IQuartzService
{
    Task AddJobToScheduler(JobRequest jobRequestMessage, CancellationToken cancellationToken);
}