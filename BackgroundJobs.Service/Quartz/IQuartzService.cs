using BackgroundJobs.Service.Interfaces;

namespace BackgroundJobs.Service.Quartz;

public interface IQuartzService
{
    Task AddJobToScheduler(IJobRequest jobRequest, CancellationToken cancellationToken);
}