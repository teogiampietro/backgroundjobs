using System.Net;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using BackgroundJobs.Infrastructure.Messages;
using BackgroundJobs.Infrastructure.Model;
using BackgroundJobs.Infrastructure.Services.Quartz;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Infrastructure.Services.Consumers;

public class SqsRequestsConsumerService : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IQuartzService _quartzService;
    private readonly string QueueName;

    public SqsRequestsConsumerService(
        IAmazonSQS sqs,
        IQuartzService quartzService,
        IOptions<AppSettings> appSettings)
    {
        _sqs = sqs;
        _quartzService = quartzService;
        QueueName = appSettings.Value.SQSQueueName;
    }

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
                // TODO: Do some logging or handling.
                continue;
            }

            foreach (var message in receiveMessageResponse.Messages)
            {
                var jobRequestMessage = JsonSerializer.Deserialize<JobRequestMessage>(message.Body)!;
                var jobRequest = new JobRequest
                {
                    Id = jobRequestMessage.Id,
                    Type = Type.GetType($"BackgroundJobs.Infrastructure.Jobs.{jobRequestMessage.Type}")!,
                    ResultsTopic = jobRequestMessage.ResultsTopic,
                    CronExpression = jobRequestMessage.CronExpression,
                    Priority = jobRequestMessage.Priority
                };

                Console.WriteLine($"Request for job {jobRequest.Id} was received from {QueueName}.");

                await _quartzService.AddJobToScheduler(jobRequest, cancellationToken);

                await _sqs.DeleteMessageAsync(getQueueUrlResponse.QueueUrl, message.ReceiptHandle, cancellationToken);
            }
        }
    }
}