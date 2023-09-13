using Quartz;

namespace BackgroundJobs.Infrastructure.Jobs;

public abstract class JobBase : IJob
{
    protected abstract Task ExecuteJob(IJobExecutionContext context);

    public async Task Execute(IJobExecutionContext context)
    {
        var logSource = context.JobDetail.Key.Name;
        try
        {
            await ExecuteJob(context);
        }
        catch (Exception e)
        {
            await Console.Out.WriteLineAsync(ErrorMessage(logSource, e));
        }
    }

    private static string ErrorMessage(string logSource, Exception e)
    {
        return $"Error: {logSource}, Message: {e.Message}, Inner: {e.InnerException}";
    }
}