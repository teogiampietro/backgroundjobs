using BackgroundJobs.Infrastructure.Interfaces;

namespace BackgroundJobs.Infrastructure.Services.Quartz;

public interface IQuartzService
{
    Task AddJobToScheduler(IJobRequestMessage jobRequestMessage, CancellationToken cancellationToken);
}