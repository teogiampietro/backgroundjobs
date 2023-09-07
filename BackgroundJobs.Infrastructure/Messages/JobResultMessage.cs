using BackgroundJobs.Infrastructure.Model;

namespace BackgroundJobs.Infrastructure.Messages;

public record JobResultMessage
{
    public required Guid JobId { get; init; }
    public required JobResult Result { get; init; }
    public string? ResultMessage { get; init; }
}