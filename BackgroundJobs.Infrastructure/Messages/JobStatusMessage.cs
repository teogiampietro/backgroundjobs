namespace BackgroundJobs.Infrastructure.Messages;

public class JobStatusMessage
{
    public required Guid JobId { get; init; }
    public string? Message { get; init; }
}