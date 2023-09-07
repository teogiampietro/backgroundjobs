namespace BackgroundJobs.LambdaExample.OutputProgress;

public class JobProgressMessage
{
    public Guid JobId { get; set; }
    public string Message { get; init; }
}