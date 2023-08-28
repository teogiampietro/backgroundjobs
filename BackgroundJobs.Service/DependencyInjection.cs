using Amazon;
using Amazon.SQS;
using BackgroundJobs.Service.Consumer;
using BackgroundJobs.Service.Model;
using BackgroundJobs.Service.Quartz;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;


namespace BackgroundJobs.Service;

public static class DependencyInjection
{
    public static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddSingleton<IQuartzService, Quartz.QuartzHostedService>();
        services.AddSingleton<IJobFactory, SingletonJobFactory>();
        services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

        //services.AddQuartz(options => options.UseMicrosoftDependencyInjectionJobFactory());
        //services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        services.AddSingleton<JobExecute>();

        // services.AddSingleton(new MyJob(
        //     type: typeof(JobExecute),
        //     cronExpression: "0/15 0/1 * 1/1 * ? *",
        //     resource: "https://aws.lambdaexampleurl.com",
        //     priority: 1));

        services.AddHostedService<SqsConsumerService>();
        services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(RegionEndpoint.SAEast1));
    }
}