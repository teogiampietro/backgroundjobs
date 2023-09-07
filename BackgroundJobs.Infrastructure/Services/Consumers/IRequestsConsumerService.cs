using BackgroundJobs.Infrastructure.Model;

namespace BackgroundJobs.Infrastructure.Services.Consumers;

public interface IRequestsConsumerService
{
    Task<QueueUrl> GetQueueUrlAsync(string queueName,
        CancellationToken cancellationToken = default(CancellationToken));

    Task<MessageResponse> ReceiveMessageAsync(MessageRequest request,
        CancellationToken cancellationToken = default(CancellationToken));

    Task<bool> DeleteMessageAsync(string queueUrl,
        string receiptHandle,
        CancellationToken cancellationToken = default(CancellationToken));
}