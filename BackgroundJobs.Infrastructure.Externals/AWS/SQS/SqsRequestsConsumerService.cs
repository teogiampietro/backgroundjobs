using System.Net;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using BackgroundJobs.Infrastructure.Model;
using BackgroundJobs.Infrastructure.Services.Consumers;
using Microsoft.Extensions.Options;
using Message = BackgroundJobs.Infrastructure.Model.Message;

namespace BackgroundJobs.Infrastructure.Externals.AWS.SQS;

public class SqsRequestsConsumerService : IRequestsConsumerService
{
    private readonly IAmazonSQS _sqs;

    public SqsRequestsConsumerService(IOptions<AwsSettings> awsSettings)
    {
        _sqs = new AmazonSQSClient(awsSettings.Value.AccessKeyId, awsSettings.Value.SecretAccessKey, RegionEndpoint.GetBySystemName(awsSettings.Value.Region));
    }

    public async Task<MessageResponse> ReceiveMessageAsync(MessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var awsReceiveMessage = await _sqs.ReceiveMessageAsync(MessageToAwsMessageRequest(request), cancellationToken);

        return new MessageResponse
        {
            HttpStatusCode = awsReceiveMessage.HttpStatusCode,
            Messages = AwsMessageToMessage(awsReceiveMessage.Messages)
        };
    }

    public async Task<QueueUrl> GetQueueUrlAsync(string queueName, CancellationToken cancellationToken)
    {
        var queueUrl = await _sqs.GetQueueUrlAsync(queueName, cancellationToken);
        return new QueueUrl(queueUrl.QueueUrl);
    }

    public async Task<bool> DeleteMessageAsync(string queueUrl,
        string receiptHandle,
        CancellationToken cancellationToken = default)
    {
        return (await _sqs.DeleteMessageAsync(queueUrl, receiptHandle, cancellationToken)).HttpStatusCode ==
               HttpStatusCode.OK;
    }

    private static ReceiveMessageRequest MessageToAwsMessageRequest(MessageRequest messageRequest)
    {
        return new ReceiveMessageRequest
        {
            AttributeNames = messageRequest.AtributesValues,
            MessageAttributeNames = messageRequest.MessageAtributesNames,
            QueueUrl = messageRequest.QueueUrl
        };
    }

    private static List<Message> AwsMessageToMessage(IEnumerable<Amazon.SQS.Model.Message> awsMessages)
    {
        return awsMessages.Select(message =>
            new Message
            {
                Body = message.Body,
                ReceiptHandle = message.ReceiptHandle,
            }).ToList();
    }
}