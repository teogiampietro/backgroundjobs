namespace BackgroundJobs.Infrastructure.Messages;

public record JobResultMessage
(
    Guid Id,
    string Status,
    string? StatusMessage
);