using BackgroundJobs.Service.Interfaces;

namespace BackgroundJobs.Service.Messages;

public class JobRequestMessage : IJobRequest
{
    public Guid Id { get; init; }
    public required Type Type { get; init; }
    public required string ResultsTopic { get; init; }
    public string? CronExpression { get; init; }
    public int Priority { get; init; } = 5;
}