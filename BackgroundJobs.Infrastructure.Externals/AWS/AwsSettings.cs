namespace BackgroundJobs.Infrastructure.Externals.AWS;

public class AwsSettings
{
    public required string AccessKeyId { get; set; }
    public required string SecretAccessKey { get; set; }
    public required string Region { get; set; }
    public required string RequestsQueueName { get; set; }
    public required string StatusQueueName { get; set; }
    public required string ResultsTopicName { get; set; }
}