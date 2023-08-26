namespace BackgroundJobs.Service.Messages;

public class JobResultMessage
{
    public required string JobKey { get; init; }
    public required string Status { get; init; }
}
