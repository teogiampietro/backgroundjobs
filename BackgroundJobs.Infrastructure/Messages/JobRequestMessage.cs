using BackgroundJobs.Infrastructure.Interfaces;

namespace BackgroundJobs.Infrastructure.Messages;

public class JobRequestMessage : IJobRequestMessage
{
    public JobRequestMessage(Guid id, Type type, string resultsTopic, int priority, string cronExpression = null)
    {
        Id = id;
        Type = type;
        ResultsTopic = resultsTopic;
        Priority = priority;
        CronExpression = cronExpression;
    }

    public Guid Id { get; set; }
    public Type Type { get; set; }
    public string TypeName { get; set; }
    public string ResultsTopic { get; set; }
    public string? CronExpression { get; set; }
    public int Priority { get; set; }
}