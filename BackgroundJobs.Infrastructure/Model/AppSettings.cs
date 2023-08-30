namespace BackgroundJobs.Infrastructure.Model;

public class AppSettings
{
    public string SNSQueueName { get; set; }
    public string SQSQueueName { get; }
}