using Quartz;

namespace BackgroundJobs.Service.Model;

public static class JobLogger 
{
    public static void Log(IJobExecutionContext context)
    {
        Console.WriteLine($"JobKey: {context.JobDetail.Key.Name}");
        Console.WriteLine($"JobKeyGroup: {context.JobDetail.Key.Group}");
        Console.WriteLine($"JobDescription: {context.JobDetail.Description}");
        
        Console.WriteLine($"TriggerKey: {context.Trigger.Key.Name}");
        Console.WriteLine($"TriggerKeyGroup: {context.Trigger.Key.Group}");
        Console.WriteLine($"TriggerDescription: {context.Trigger.Description}");
        Console.WriteLine($"TriggerPriority: {context.Trigger.Priority}");
    }
}