namespace BackgroundJobs.Service.Interfaces;

public interface IJobResult
{
    Guid Id { get; init; }
    string Status { get; init; }
    string? StatusMessage { get; init; }
}