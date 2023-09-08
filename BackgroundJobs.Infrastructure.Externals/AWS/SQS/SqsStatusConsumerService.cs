using System.Net;
using System.Text.Json;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using BackgroundJobs.Infrastructure.Messages;
using BackgroundJobs.Infrastructure.Model;
using BackgroundJobs.Infrastructure.Services.Publishers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Infrastructure.Externals.AWS.SQS;

public class SqsStatusConsumerService : BackgroundService
{
    private readonly IResultsPublisherService _resultsPublisherService;
    private readonly IAmazonSQS _sqs;
    private readonly string _queueName;

    public SqsStatusConsumerService(IResultsPublisherService resultsPublisherService, IOptions<AwsSettings> awsSettings)
    {
        _resultsPublisherService = resultsPublisherService;
        _sqs = new AmazonSQSClient(awsSettings.Value.AccessKeyId, awsSettings.Value.SecretAccessKey, RegionEndpoint.GetBySystemName(awsSettings.Value.Region));
        _queueName = awsSettings.Value.StatusQueueName;
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
                var jobStatusMessage = JsonSerializer.Deserialize<JobStatusMessage>(message.Body);

                Console.WriteLine($"Status for job {jobStatusMessage.JobId} was received from {_queueName} with message {jobStatusMessage.Message}.");

                var jobResultMessage = new JobResultMessage
                {
                    JobId = jobStatusMessage.JobId,
                    Result = JobResult.Ok,
                    ResultMessage = jobStatusMessage.Message
                };

                await _resultsPublisherService.Publish(jobResultMessage);

                await _sqs.DeleteMessageAsync(getQueueUrlResponse.QueueUrl, message.ReceiptHandle, cancellationToken);
            }
        }
    }
}