namespace ChessAnalyzerApi.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        => (_next, _logger) = (next, logger);

    public async Task InvokeAsync(HttpContext context)
    {
        var requestDateTime = DateTime.Now;
        var clientIp = context.Request.HttpContext.Connection.RemoteIpAddress;
        var endPoint = context.Request.Path;

        // логгирование начала запроса
        _logger.LogTrace("requestDateTime '{0}', clientIp '{1}', endPoint '{2}'", requestDateTime, clientIp, endPoint);
      
        await _next(context);

        // логгирование завершения обработки запроса
        _logger.LogTrace($"Request: {context.Request.Path} - {context.Request.Method} completed with status code: {context.Response.StatusCode}");
    }
}

// ниже старыый вариант

////public class LoggingMiddleware : IMiddleware (его видимость можно настроить как scoped/trasient)
////{
////    private readonly ILogger<LoggingMiddleware> _logger;

////    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
////    {
////        _logger = logger;
////    }

////    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
////    {

////        // Логгирование начала обработки запроса
////        _logger.LogInformation($"Request: {context.Request.Path} - {context.Request.Method}");

////        try
////        {
////            await next(context); // вызов следующего middleware в цепочке
////        }
////        catch (Exception ex)
////        {
////            // Логгирование ошибки
////            _logger.LogError(ex, $"An error occurred while processing the request: {context.Request.Path}");
////            throw;
////        }

////        // Логгирование завершения обработки запроса
////        _logger.LogInformation($"Request: {context.Request.Path} - {context.Request.Method} completed with status code: {context.Response.StatusCode}");
////    }

////    #region Методы для форматированного логгирования
////    private Dictionary<string, string> FormatHeaders(IHeaderDictionary headers)
////    {
////        Dictionary<string, string> pairs = new();
////        foreach (var header in headers)
////        {
////            pairs.Add(header.Key, header.Value);
////        }
////        return pairs;
////    }

////    private List<KeyValuePair<string, string>> FormatQueries(string queryString)
////    {
////        // переписать этот метод!!! 
////        List<KeyValuePair<string, string>> pairs = new();
////        string key, value;
////        foreach (var query in queryString.TrimStart('?').Split("&"))
////        {
////            var items = query.Split("=");
////            key = items.Length >= 1 ? items[0] : string.Empty;
////            value = items.Length >= 2 ? items[1] : string.Empty;
////            if (!string.IsNullOrEmpty(key))
////            {
////                pairs.Add(new KeyValuePair<string, string>(key, value));
////            }
////        }
////        return pairs;
////    }

////    private async Task<string> ReadBodyFromRequest(HttpRequest request)
////    {
////        // Ensure the request's body can be read multiple times (for the next middlewares in the pipeline).
////        request.EnableBuffering();
////        using var streamReader = new StreamReader(request.Body, leaveOpen: true);
////        var requestBody = await streamReader.ReadToEndAsync();
////        // Reset the request's body stream position for next middleware in the pipeline.
////        request.Body.Position = 0;
////        return requestBody;
////    }
////    #endregion
////}