namespace BackgroundJobs.Infrastructure.Model;

public class QueueUrl
{
    public QueueUrl(string queueUrlName)
    {
        QueueUrlName = queueUrlName;
    }
    public string  QueueUrlName{ get; init; }
}