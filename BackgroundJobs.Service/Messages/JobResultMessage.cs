namespace BackgroundJobs.Service.Messages;

public class JobResultMessage
{
    public Guid Id { get; init; }
    public required string Status { get; init; }
    public string? StatusMessage { get; init; }
}