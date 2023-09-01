using System.Net;
using System.Text.Json;
using BackgroundJobs.Infrastructure.Messages;
using BackgroundJobs.Infrastructure.Model;
using BackgroundJobs.Infrastructure.Services.Quartz;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BackgroundJobs.Infrastructure.Services.Consumers;

public class MessagesReceiver : BackgroundService
{
    private readonly IInputConsumer _inputConsumer;
    private readonly IQuartzService _quartzService;
    private readonly string QueueName;

    private readonly List<string> _messageAttributeNames = new() { "All" };
    private readonly List<string> _attributeNames = new() { "All" };

    public MessagesReceiver(IInputConsumer inputConsumer, IQuartzService quartzService,
        IOptions<AppSettings> appSettings)
    {
        _inputConsumer = inputConsumer;
        _quartzService = quartzService;
        QueueName = appSettings.Value.InputId;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var getQueueUrlResponse = await _inputConsumer.GetQueueUrlAsync(QueueName, cancellationToken);
        var receiveMessageRequest = new MessageRequest
        {
            QueueUrl = getQueueUrlResponse.QueueUrlName,
            MessageAtributesNames = _messageAttributeNames,
            AtributesValues = _attributeNames
        };

        while (!cancellationToken.IsCancellationRequested)
        {
            var receiveMessageResponse =
                await _inputConsumer.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);
            if (receiveMessageResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Error receiving messages. {receiveMessageResponse.HttpStatusCode}");
                continue;
            }

            foreach (var message in receiveMessageResponse.Messages)
            {
                var jobRequestMessage = JsonSerializer.Deserialize<JobRequestMessage>(message.Body);
                var jobRequest = new JobRequestMessage
                (
                    jobRequestMessage.Id,
                    Type.GetType($"BackgroundJobs.Infrastructure.Jobs.{jobRequestMessage.TypeName}")!,
                    jobRequestMessage.ResultsTopic,
                    jobRequestMessage.Priority,
                    jobRequestMessage.CronExpression
                );

                Console.WriteLine($"Request for job {jobRequest.Id} was received from {QueueName}.");

                await _quartzService.AddJobToScheduler(jobRequest, cancellationToken);

                await _inputConsumer.DeleteMessageAsync(getQueueUrlResponse.QueueUrlName, message.ReceiptHandle,
                    cancellationToken);
            }
        }
    }
}