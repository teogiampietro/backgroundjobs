namespace BackgroundJobs.Service.Model;

public class MyJob
{
    public MyJob(Type type, string cronExpression, string resource, int priority = 5)
    {
        Type = type;
        CronExpression = cronExpression;
        Resource = resource;
        Priority = priority;
    }
    public Type Type { get; }
    public int Priority { get; }
    public string CronExpression { get; }
    public string Resource { get; }
}