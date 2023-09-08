namespace BackgroundJobs.Infrastructure.Model;

public enum JobResult
{
    Ok,
    CompleteWithErrors,
    InProcess,
    Fail
}