using BackgroundJobs.Service.Model;
using Quartz;

namespace BackgroundJobs.Service.Quartz;

public interface IQuartzService
{
    Task AddJobToScheduler(MyJob myJob, CancellationToken cancellationToken);
}