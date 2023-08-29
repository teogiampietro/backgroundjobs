using BackgroundJobs.Service.Interfaces;
using BackgroundJobs.Service.Messages;

namespace BackgroundJobs.Service.Model;

public class JobRequest : IJobRequest
{
    public Guid Id { get; init; }
    public required Type Type { get; init; }
    public required string ResultsTopic { get; init; }
    public string? CronExpression { get; init; }
    public int Priority { get; init; } = 5;
}