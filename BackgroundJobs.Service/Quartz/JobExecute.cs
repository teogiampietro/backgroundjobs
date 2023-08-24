using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using BackgroundJobs.Service.Model;
using Quartz;

namespace BackgroundJobs.Service.Quartz;

public class JobExecute : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        // Do something: log data, call a Lambda, HTTP request, etc.
        JobLogger.Log(context);

        string jobKey = context.JobDetail.Key.ToString();
        
        JobResultMessage resultMessage = new()
        {
            JobKey = jobKey,
            Status = "OK"
        };
        
        AmazonSimpleNotificationServiceClient snsClient = new();
        
        Topic topicArnResponse = await snsClient.FindTopicAsync("background-jobs-results-topic");
        
        PublishRequest publishRequest = new()
        {
            TopicArn = topicArnResponse.TopicArn,
            Message = JsonSerializer.Serialize(resultMessage)
        };
        
        await snsClient.PublishAsync(publishRequest);
    }
}