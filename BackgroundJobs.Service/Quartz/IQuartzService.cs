using BackgroundJobs.Service.Model;
using Quartz;

namespace BackgroundJobs.Service.Quartz;

public interface IQuartzService
{
    Task AddJobToScheduler(CancellationToken cancellationToken, MyJob myJob);
}