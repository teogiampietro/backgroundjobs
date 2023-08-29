using BackgroundJobs.Service.Interfaces;
using BackgroundJobs.Service.Model;

namespace BackgroundJobs.Service.Quartz;

public interface IQuartzService
{
    Task AddJobToScheduler(IJobRequest jobRequest, CancellationToken cancellationToken);
}