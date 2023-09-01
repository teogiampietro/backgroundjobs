using BackgroundJobs.Infrastructure.Model;

namespace BackgroundJobs.Infrastructure.Interfaces;

public interface IJobResultMessage
{
    Guid Id { get; init; }
    StatusResults Status { get; init; }
    string? StatusMessage { get; init; }
}