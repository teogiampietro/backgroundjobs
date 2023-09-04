using Amazon;
using Amazon.SQS;
using BackgroundJobs.Infrastructure.Externals.AWS.SNS;
using BackgroundJobs.Infrastructure.Externals.AWS.SQS;
using BackgroundJobs.Infrastructure.Services.Consumers;
using BackgroundJobs.Infrastructure.Services.Publishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BackgroundJobs.Infrastructure.Externals.AWS;

public static class DependencyInjection
{
    public static void AddAwsServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AwsSettings>(configuration.GetSection("AwsSettings"));

        services.AddSingleton<IOutputResultPublisher, SnsPublisher>();
        services.AddSingleton<IInputConsumer, SqsConsumer>();
    }
}