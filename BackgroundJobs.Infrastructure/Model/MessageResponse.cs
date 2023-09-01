using System.Net;
namespace BackgroundJobs.Infrastructure.Model;

public class MessageResponse
{
    public HttpStatusCode HttpStatusCode { get; set; }
    public List<Message> Messages { get; set; }
    

}

public class Message
{
    public string Body { get; set; }
    public string ReceiptHandle { get; set; }
}