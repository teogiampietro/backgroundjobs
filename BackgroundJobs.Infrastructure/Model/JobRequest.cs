namespace BackgroundJobs.Infrastructure.Model;

public class JobRequest
{
    public required Guid JobId { get; init; }
    public required Type JobType { get; init; }
    public required string ResultsTopicName { get; init; }
    public string? CronExpression { get; init; }
    public int Priority { get; init; }
}