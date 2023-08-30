namespace BackgroundJobs.Infrastructure.Interfaces;

public interface IJobRequest
{
    Guid Id { get; init; }
    Type Type { get; init; }
    string ResultsTopic { get; init; }
    string? CronExpression { get; init; }
    int Priority { get; init; }
}