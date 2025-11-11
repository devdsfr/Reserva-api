using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ReservaSalas.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var requestData = JsonSerializer.Serialize(request);

            _logger.LogInformation("----- Request: {Name} {@RequestData} at {DateTime}",
                requestName, requestData, DateTime.UtcNow);

            var response = await next();

            var responseData = JsonSerializer.Serialize(response);

            _logger.LogInformation("----- Response: {Name} {@ResponseData} at {DateTime}",
                requestName, responseData, DateTime.UtcNow);

            return response;
        }
    }
}
