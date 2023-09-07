using BackgroundJobs.Infrastructure.Model;

namespace BackgroundJobs.Infrastructure.Messages;

public record JobResultMessage
{
    public required Guid JobId { get; init; }
    public required StatusResults Status { get; init; }
    public string? StatusMessage { get; init; }
}