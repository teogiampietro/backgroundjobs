using Quartz;

namespace BackgroundJobs.Service.Model;

public static class JobLogger 
{
    public static void LogContextJob(IJobExecutionContext context)
    {
        Console.WriteLine($"JobKey: {context.JobDetail.Key.Name}");
        Console.WriteLine($"JobKeyGroup: {context.JobDetail.Key.Group}");
        Console.WriteLine($"JobDescription: {context.JobDetail.Description}");
        
        Console.WriteLine($"TriggerKey: {context.Trigger.Key.Name}");
        Console.WriteLine($"TriggerKeyGroup: {context.Trigger.Key.Group}");
        Console.WriteLine($"TriggerDescription: {context.Trigger.Description}");
        Console.WriteLine($"TriggerPriority: {context.Trigger.Priority}");
    }
    public static void LogMyJob(MyJob context)
    {
        Console.WriteLine($"MyJob: {context.Type}");
        Console.WriteLine($"MyJob: {context.CronExpression}");
        Console.WriteLine($"MyJob: {context.Priority}");
        Console.WriteLine($"MyJob: {context.Resource}");
      

    }
}