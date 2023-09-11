using BackgroundJobs.Infrastructure.Services.Publishers;
using Quartz;

namespace BackgroundJobs.Infrastructure.Jobs;

public class LambdaJob : IJob
{
    private readonly HttpClient _httpClient;

    private readonly IResultsPublisherService _resultsPublisherService;

    public LambdaJob(IResultsPublisherService resultsPublisherService)
    {
        _resultsPublisherService = resultsPublisherService;
        _httpClient = new HttpClient
            { BaseAddress = new Uri("https://zy4si9954h.execute-api.us-east-2.amazonaws.com/default/") };
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _httpClient.GetAsync("LambdaToSnsExample");
        await _resultsPublisherService.Publish(context);
    }
}