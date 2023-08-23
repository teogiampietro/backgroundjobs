using BackgroundJobs.Infrastructure.Model;
using BackgroundJobs.Infrastructure.Service;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace BackgroundJobs.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddHostedService<Service.QuartzHostedService>();
        services.AddSingleton<IJobFactory, SingletonJobFactory>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

        services.AddSingleton<JobExecute>();
        services.AddSingleton(new MyJob(
            type: typeof(JobExecute),
            cronExpression: "0/30 0/1 * 1/1 * ? *",
            resource: "https://aws.lambdaexampleurl.com",
            priority: 1));
    }
}