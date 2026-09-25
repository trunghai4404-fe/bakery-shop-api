namespace Shared.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Starting request execution: {RequestName}", requestName);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            logger.LogWarning("Long-running request detected: {RequestName} took {ElapsedMilliseconds} ms", requestName, elapsedMilliseconds);
        }
        else
        {
            logger.LogInformation("Completed request execution: {RequestName} in {ElapsedMilliseconds} ms", requestName, elapsedMilliseconds);
        }

        return response;
    }
}
