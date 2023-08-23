using Microsoft.Extensions.Logging;
using Quartz;

namespace BackgroundJobs.Infrastructure;

[DisallowConcurrentExecution]
public class LoggingBackgroundJob : IJob
{
    private readonly ILogger<LoggingBackgroundJob> _logger;

    public LoggingBackgroundJob(ILogger<LoggingBackgroundJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        //Do something, call a Lambda, http request, etc
        _logger.LogInformation("{UtcNow}", DateTime.UtcNow);
        return Task.CompletedTask;
    }
}