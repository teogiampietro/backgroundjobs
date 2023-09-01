using Amazon;
using Amazon.SQS;
using BackgroundJobs.Infrastructure.Externals.AWS.SNS;
using BackgroundJobs.Infrastructure.Externals.AWS.SQS;
using BackgroundJobs.Infrastructure.Services.Consumers;
using BackgroundJobs.Infrastructure.Services.Publishers;
using Microsoft.Extensions.DependencyInjection;

namespace BackgroundJobs.Infrastructure.Externals.AWS;

public static class DependencyInjection
{
    public static void AddAwsServices(this IServiceCollection services)
    {
        services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(RegionEndpoint.SAEast1));
        services.AddSingleton<IOutputResultPublisher, SnsPublisher>();
        services.AddSingleton<IInputConsumer, SqsConsumer>();

    }
}