using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using BackgroundJobs.LambdaExample.OutputProgress;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BackgroundJobs.LambdaExample;

public class LambdaToSnsExample
{
    public LambdaToSnsExample()
    {
    }

    private readonly int delayFiveSeconds = 5000;

    public async Task<APIGatewayProxyResponse> Send(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var _sns = new SnsProgress();

        for (var i = 1; i < 3; i++)
        {
            Task.Delay(delayFiveSeconds);

            var jobProgressMessage = new JobProgressMessage
            {
                JobId = Guid.NewGuid(),
                Message = $"Job number {i}"
            };
            await _sns.Publish(jobProgressMessage);
            context.Logger.LogInformation($"Message sent. Job Number {i} | JobId: {jobProgressMessage.JobId}.");
        }

        return new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = "All jobs completed successfully.",
            Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
        };
    }
}