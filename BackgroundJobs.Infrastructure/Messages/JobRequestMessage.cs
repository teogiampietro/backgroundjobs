namespace BackgroundJobs.Infrastructure.Messages;

public class JobRequestMessage
{
    public Guid Id { get; init; }
    public required string Type { get; set; }
    public required string ResultsTopic { get; init; }
    public string? CronExpression { get; init; }
    public int Priority { get; init; }
}