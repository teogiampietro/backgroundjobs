using BackgroundJobs.Infrastructure.Interfaces;

namespace BackgroundJobs.Infrastructure.Model;

public class JobRequest : IJobRequest
{
    public Guid Id { get; init; }
    public required Type Type { get; init; }
    public required string ResultsTopic { get; init; }
    public string? CronExpression { get; init; }
    public int Priority { get; init; } = 5;
}