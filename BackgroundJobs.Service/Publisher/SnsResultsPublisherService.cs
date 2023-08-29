using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using BackgroundJobs.Service.Messages;
using Quartz;
using System.Text.Json;

namespace BackgroundJobs.Service.Publisher;

public static class SnsResultsPublisherService
{
    public static async Task Publish(IJobExecutionContext context) {
        var jobId = context.JobDetail.Key.Name;
        
        var jobDataMap = context.MergedJobDataMap;

        var topicName = jobDataMap.GetString("ResultsTopic")!;

        var jobResultMessage = new JobResultMessage
        {
            Id = Guid.Parse(jobId),
            Status = "OK",
            StatusMessage = "Job was successfully executed."
        };

        var snsClient = new AmazonSimpleNotificationServiceClient();

        var topic = await snsClient.FindTopicAsync(topicName);
        
        var publishRequest = new PublishRequest
        {
            TopicArn = topic.TopicArn,
            Message = JsonSerializer.Serialize(jobResultMessage)
        };

        await snsClient.PublishAsync(publishRequest);
        
        await Console.Out.WriteLineAsync($"Results for job {context.JobDetail.Key.Name} were published on {topicName}.");
    }
}