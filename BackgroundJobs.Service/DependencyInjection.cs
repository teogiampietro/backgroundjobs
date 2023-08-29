using Amazon;
using Amazon.SQS;
using BackgroundJobs.Service.Consumer;
using BackgroundJobs.Service.Quartz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;


namespace BackgroundJobs.Service;

public static class DependencyInjection
{
    public static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddSingleton<IJobFactory, SingletonJobFactory>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
        
        services.AddSingleton<Quartz.QuartzHostedService>();
        services.AddSingleton<IQuartzService>(p => p.GetRequiredService<Quartz.QuartzHostedService>());
        services.AddSingleton<IHostedService>(p => p.GetRequiredService<Quartz.QuartzHostedService>());

        // Ad job types as a singleton.
        services.AddSingleton<LoggingJob>();

        services.AddHostedService<SqsRequestsConsumerService>();
        services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(RegionEndpoint.SAEast1));
    }
}