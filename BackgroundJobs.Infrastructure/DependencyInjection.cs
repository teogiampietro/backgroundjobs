using BackgroundJobs.Infrastructure.Jobs;
using BackgroundJobs.Infrastructure.Services.JobsReceiver;
using BackgroundJobs.Infrastructure.Services.Quartz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzHostedService = BackgroundJobs.Infrastructure.Services.Quartz.QuartzHostedService;

namespace BackgroundJobs.Infrastructure;

public static class DependencyInjection
{
    public static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddSingleton<IJobFactory, SingletonJobFactory>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        services.AddSingleton<QuartzHostedService>();
        services.AddSingleton<IQuartzService>(p => p.GetRequiredService<QuartzHostedService>());
        services.AddSingleton<IHostedService>(p => p.GetRequiredService<QuartzHostedService>());

        services.AddHostedService<JobsReceiver>();
        // Add job types as a singleton.
        services.AddSingleton<LoggingJob>();
        services.AddSingleton<SlowLoggingJob>();
    }
}