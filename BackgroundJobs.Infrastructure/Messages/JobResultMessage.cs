using BackgroundJobs.Infrastructure.Interfaces;
using BackgroundJobs.Infrastructure.Model;

namespace BackgroundJobs.Infrastructure.Messages;

public class JobResultMessage : IJobResultMessage
{
    public JobResultMessage(Guid Id, StatusResults Status, string? StatusMessage = null)
    {
        this.Id = Id;
        this.Status = Status;
        this.StatusMessage = StatusMessage;
    }

    public Guid Id { get; init; }
    public StatusResults Status { get; init; }
    public string? StatusMessage { get; init; }
}