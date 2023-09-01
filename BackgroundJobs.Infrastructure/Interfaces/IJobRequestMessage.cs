namespace BackgroundJobs.Infrastructure.Interfaces;

public interface IJobRequestMessage
{
    Guid Id { get; set; }
    Type Type { get; set; }
    string TypeName { get; set; }
    string ResultsTopic { get; set; }
    string? CronExpression { get; set; }
    int Priority { get; set; }
}