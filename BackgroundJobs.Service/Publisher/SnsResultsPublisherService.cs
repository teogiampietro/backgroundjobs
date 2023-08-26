using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using BackgroundJobs.Service.Messages;
using Quartz;
using System.Text.Json;

namespace BackgroundJobs.Service.Publisher;

public static class SnsResultsPublisherService
{
    private static readonly string topicName = "background-jobs-results-topic";

    public static async Task Publish(IJobExecutionContext context) {
        string jobKey = context.JobDetail.Key.ToString();

        JobResultMessage resultMessage = new()
        {
            JobKey = jobKey,
            Status = "OK"
        };

        AmazonSimpleNotificationServiceClient snsClient = new();

        Topic topicArnResponse = await snsClient.FindTopicAsync(topicName);

        PublishRequest publishRequest = new()
        {
            TopicArn = topicArnResponse.TopicArn,
            Message = JsonSerializer.Serialize(resultMessage)
        };

        await snsClient.PublishAsync(publishRequest);
    }
}
