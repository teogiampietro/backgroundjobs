﻿using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using BackgroundJobs.Infrastructure.Messages;
using Quartz;

namespace BackgroundJobs.Infrastructure.Services.Publishers;

public static class SnsResultsPublisherService
{
    public static async Task Publish(IJobExecutionContext context) {
        var jobId = context.JobDetail.Key.Name;
        
        var jobDataMap = context.MergedJobDataMap;

        var topicName = jobDataMap.GetString("ResultsTopic")!;

        var jobResultMessage = new JobResultMessage
        (
            Guid.Parse(jobId),
            "OK",
            "Job was successfully executed."
        );

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