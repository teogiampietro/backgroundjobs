namespace BackgroundJobs.Infrastructure.Messages;

public record JobRequestMessage
{
    public required Guid JobId { get; init; }
    public required string JobType { get; init; }
    public required string ResultsTopicName { get; init; }
    public string? CronExpression { get; init; }
    public int Priority { get; init; }
}