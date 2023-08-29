using System.Net;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using BackgroundJobs.Service.Messages;
using BackgroundJobs.Service.Quartz;
using Microsoft.Extensions.Hosting;

namespace BackgroundJobs.Service.Consumer;

public class SqsRequestsConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IQuartzService _quartzService;

    public SqsRequestsConsumerService(IAmazonSQS sqs, IQuartzService quartzService)
    {
        _sqs = sqs;
        _quartzService = quartzService;
    }
    
    private const string QueueName = "background-jobs-requests-queue";
    private readonly List<string> _messageAttributeNames = new() { "All" };
    private readonly List<string> _attributeNames = new() { "All" };

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var getQueueUrlResponse = await _sqs.GetQueueUrlAsync(QueueName, cancellationToken);
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
                // TODO: Do some logging or handling
                continue;
            }

            foreach (var message in receiveMessageResponse.Messages)
            {
                // TODO: Create the job from the request and add it to the scheduler.
                var jobRequestMessage = new JobRequestMessage
                {
                    Id = Guid.NewGuid(),
                    Type = typeof(LoggingJob),
                    ResultsTopic = "background-jobs-results-topic",
                    Priority = 3
                };

                await _quartzService.AddJobToScheduler(jobRequestMessage, cancellationToken);
                
                // TODO: Verify the job was correctly scheduled and delete the message.
                if (false)
                    await _sqs.DeleteMessageAsync(getQueueUrlResponse.QueueUrl, message.ReceiptHandle, cancellationToken);
                
                Console.WriteLine($"Request for job {jobRequestMessage.Id} received from {QueueName}.");
            }
        }
    }
}