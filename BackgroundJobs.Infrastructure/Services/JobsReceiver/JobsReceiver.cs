using System.Net;
using System.Text.Json;
using BackgroundJobs.Infrastructure.Messages;
using BackgroundJobs.Infrastructure.Model;
using BackgroundJobs.Infrastructure.Services.Consumers;
using BackgroundJobs.Infrastructure.Services.Quartz;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Infrastructure.Services.JobsReceiver;

public class JobsReceiver : BackgroundService
{
    private readonly IRequestsConsumerService _requestsConsumerService;
    private readonly IQuartzService _quartzService;
    private readonly string _queueName;

    private readonly List<string> _messageAttributeNames = new() { "All" };
    private readonly List<string> _attributeNames = new() { "All" };

    public JobsReceiver(IRequestsConsumerService requestsConsumerService, IQuartzService quartzService,
        IOptions<AppSettings> appSettings)
    {
        _requestsConsumerService = requestsConsumerService;
        _quartzService = quartzService;
        _queueName = appSettings.Value.InputId;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var getQueueUrlResponse = await _requestsConsumerService.GetQueueUrlAsync(_queueName, cancellationToken);
        var receiveMessageRequest = new MessageRequest
        {
            QueueUrl = getQueueUrlResponse.QueueUrlName,
            MessageAtributesNames = _messageAttributeNames,
            AtributesValues = _attributeNames
        };

        while (!cancellationToken.IsCancellationRequested)
        {
            var receiveMessageResponse =
                await _requestsConsumerService.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);
            if (receiveMessageResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Error receiving messages. {receiveMessageResponse.HttpStatusCode}");
                continue;
            }

            foreach (var message in receiveMessageResponse.Messages)
            {
                var jobRequestMessage = JsonSerializer.Deserialize<JobRequestMessage>(message.Body);
                var jobRequest = new JobRequest
                {
                    JobId = jobRequestMessage.JobId,
                    JobType = Type.GetType($"BackgroundJobs.Infrastructure.Jobs.{jobRequestMessage.JobType}")!,
                    ResultsTopicName = jobRequestMessage.ResultsTopicName,
                    CronExpression = jobRequestMessage.CronExpression,
                    Priority = jobRequestMessage.Priority
                };

                Console.WriteLine($"Request for job {jobRequest.JobId} was received from {_queueName}.");

                await _quartzService.AddJobToScheduler(jobRequest, cancellationToken);

                await _requestsConsumerService.DeleteMessageAsync(getQueueUrlResponse.QueueUrlName, message.ReceiptHandle,
                    cancellationToken);
            }
        }
    }
}