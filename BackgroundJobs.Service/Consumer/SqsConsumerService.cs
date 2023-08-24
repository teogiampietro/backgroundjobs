using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;

namespace BackgroundJobs.Service.Consumer;

public class SqsConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;

    public SqsConsumerService(IAmazonSQS sqs)
    {
        _sqs = sqs;
    }

    private const string QueueName = "background-jobs-requests-queue";
    private readonly List<string> _messageAttributeNames = new() { "All" };

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var queueUrl = await _sqs.GetQueueUrlAsync(QueueName, cancellationToken);
        var receiveRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl.QueueUrl,
            MessageAttributeNames = _messageAttributeNames,
            AttributeNames = _messageAttributeNames
        };
        while (!cancellationToken.IsCancellationRequested)
        {
            var messageResponse = await _sqs.ReceiveMessageAsync(receiveRequest, cancellationToken);
            if (messageResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                //TODO: Do some logging or handling
                continue;
            }

            foreach (var message in messageResponse.Messages)
            {
                //TODO: While SQS receive queue, we have to create jobs and add it to the scheduler. 
                var jobCreatorResult = false;
                if (jobCreatorResult)
                    RemoveMessageFromQueue(cancellationToken, queueUrl, message);
            }
        }
    }

    private void RemoveMessageFromQueue(CancellationToken cancellationToken, GetQueueUrlResponse queueUrl,
        Message message) => _sqs.DeleteMessageAsync(queueUrl.QueueUrl, message.ReceiptHandle, cancellationToken);
}