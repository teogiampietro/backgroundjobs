namespace BackgroundJobs.LambdaExample.OutputProgress;

public class JobStatusMessage
{
    public Guid JobId { get; init; }
    public string? Message { get; set; }
}