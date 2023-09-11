using System.Net;
using System.Text.Json;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using BackgroundJobs.Infrastructure.Messages;
using BackgroundJobs.Infrastructure.Model;
using BackgroundJobs.Infrastructure.Services.Quartz;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Infrastructure.Externals.AWS.SQS;

public class SqsRequestsConsumerService : BackgroundService
{
    private readonly IJobSchedulerService _jobSchedulerService;
    private readonly IAmazonSQS _sqs;
    private readonly string _queueName;

    public SqsRequestsConsumerService(IJobSchedulerService jobSchedulerService, IOptions<AwsSettings> awsSettings)
    {
        _jobSchedulerService = jobSchedulerService;
        _sqs = new AmazonSQSClient(awsSettings.Value.AccessKeyId, awsSettings.Value.SecretAccessKey, RegionEndpoint.GetBySystemName(awsSettings.Value.Region));
        _queueName = awsSettings.Value.RequestsQueueName;
    }
    
    private readonly List<string> _messageAttributeNames = new() { "All" };
    private readonly List<string> _attributeNames = new() { "All" };

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var getQueueUrlResponse = await _sqs.GetQueueUrlAsync(_queueName, cancellationToken);
        
        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = getQueueUrlResponse.QueueUrl,
            MessageAttributeNames = _messageAttributeNames,
            AttributeNames = _attributeNames
        };

        while (!cancellationToken.IsCancellationRequested)
        {
            var receiveMessageResponse = await _sqs.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);
            
            if (receiveMessageResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Error receiving messages. {receiveMessageResponse.HttpStatusCode}");
                continue;
            }
            
            foreach (var message in receiveMessageResponse.Messages)
            {
                var jobRequestMessage = JsonSerializer.Deserialize<JobRequestMessage>(message.Body);
                
                Console.WriteLine($"Request for job {jobRequestMessage!.JobId} was received from {_queueName}.");
                
                var jobType = _jobSchedulerService.GetJobType(jobRequestMessage.JobType);

                if (jobType is null)
                {
                    Console.WriteLine("Job type is not supported.");
                    continue;
                }
                
                var jobRequest = new JobRequest
                {
                    JobId = jobRequestMessage.JobId,
                    JobType = jobType,
                    ResultsTopicName = jobRequestMessage.ResultsTopicName,
                    CronExpression = jobRequestMessage.CronExpression,
                    Priority = jobRequestMessage.Priority
                };

                await _jobSchedulerService.AddJobToScheduler(jobRequest, cancellationToken);

                await _sqs.DeleteMessageAsync(getQueueUrlResponse.QueueUrl, message.ReceiptHandle, cancellationToken);
            }
        }
    }
}