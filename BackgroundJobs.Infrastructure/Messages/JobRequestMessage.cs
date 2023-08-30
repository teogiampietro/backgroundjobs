namespace BackgroundJobs.Infrastructure.Messages;

public record JobRequestMessage
(
    Guid Id,
    string Type,
    string ResultsTopic,
    string? CronExpression,
    int Priority
);