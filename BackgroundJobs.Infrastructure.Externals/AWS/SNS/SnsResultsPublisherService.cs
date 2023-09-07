using System.Text.Json;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using BackgroundJobs.Infrastructure.Messages;
using BackgroundJobs.Infrastructure.Model;
using BackgroundJobs.Infrastructure.Services.Publishers;
using Microsoft.Extensions.Options;
using Quartz;

namespace BackgroundJobs.Infrastructure.Externals.AWS.SNS;

public class SnsResultsPublisherService : IResultsPublisherService
{
    private readonly AmazonSimpleNotificationServiceClient _sns;
    private readonly string _topicName;

    public SnsResultsPublisherService(IOptions<AwsSettings> awsSettings)
    {
        _sns = new AmazonSimpleNotificationServiceClient(awsSettings.Value.AccessKeyId, awsSettings.Value.SecretAccessKey, RegionEndpoint.GetBySystemName(awsSettings.Value.Region));
        _topicName = awsSettings.Value.ResultsTopicName;
    }

    public async Task Publish(IJobExecutionContext context)
    {
        var jobId = context.JobDetail.Key.Name;

        var jobDataMap = context.MergedJobDataMap;

        var topicName = jobDataMap.GetString("ResultsTopic")!;

        var jobResultMessage = new JobResultMessage
        {
            JobId = Guid.Parse(jobId),
            Result = JobResult.Ok
        };

        var topic = await _sns.FindTopicAsync(topicName);

        var publishRequest = new PublishRequest
        {
            TopicArn = topic.TopicArn,
            Message = JsonSerializer.Serialize(jobResultMessage)
        };

        await _sns.PublishAsync(publishRequest);

        await Console.Out.WriteLineAsync($"Results for job {context.JobDetail.Key.Name} were published on {topicName}.");
    }

    public async Task Publish(JobResultMessage jobResultMessage)
    {
        var topic = await _sns.FindTopicAsync(_topicName);
        
        var publishRequest = new PublishRequest
        {
            TopicArn = topic.TopicArn,
            Message = JsonSerializer.Serialize(jobResultMessage)
        };

        await _sns.PublishAsync(publishRequest);

        await Console.Out.WriteLineAsync($"Results for job {jobResultMessage.JobId} were published on {_topicName}.");
    }
}