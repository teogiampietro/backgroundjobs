using BackgroundJobs.Infrastructure.Jobs;
using BackgroundJobs.Infrastructure.Services.Quartz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace BackgroundJobs.Infrastructure;

public static class DependencyInjection
{
    public static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddSingleton<IJobFactory, SingletonJobFactory>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        
        services.AddSingleton<JobSchedulerService>();
        services.AddSingleton<IJobSchedulerService>(p => p.GetRequiredService<JobSchedulerService>());
        services.AddSingleton<IHostedService>(p => p.GetRequiredService<JobSchedulerService>());

        // Add job types as a singleton.
        services.AddSingleton<LoggingJob>();
        services.AddSingleton<SlowLoggingJob>();
        services.AddSingleton<LambdaJob>();
    }
}