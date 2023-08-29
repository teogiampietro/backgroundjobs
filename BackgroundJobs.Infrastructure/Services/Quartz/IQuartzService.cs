using BackgroundJobs.Infrastructure.Interfaces;

namespace BackgroundJobs.Infrastructure.Services.Quartz;

public interface IQuartzService
{
    Task AddJobToScheduler(IJobRequest jobRequest, CancellationToken cancellationToken);
}