namespace BackgroundJobs.Infrastructure.Model;

public class MessageRequest
{
    public string QueueUrl { get; set; }
    public List<string> MessageAtributesNames { get; set; }
    public List<string> AtributesValues { get; set; }
}