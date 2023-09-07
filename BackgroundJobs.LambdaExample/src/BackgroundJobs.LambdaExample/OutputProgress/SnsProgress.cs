using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace BackgroundJobs.LambdaExample.OutputProgress;

public class SnsProgress : IOutputProgressPublisher
{
    private readonly AmazonSimpleNotificationServiceClient _sns = new();

    public async Task Publish(JobStatusMessage jobProgressMessage)
    {
        var topic = await _sns.FindTopicAsync("background-jobs-status-topic");

        var publishRequest = new PublishRequest
        {
            TopicArn = topic.TopicArn,
            Message = JsonSerializer.Serialize(jobProgressMessage)
        };
        await _sns.PublishAsync(publishRequest);
    }
}