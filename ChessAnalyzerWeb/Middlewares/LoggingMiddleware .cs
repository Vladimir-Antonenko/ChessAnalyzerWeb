namespace ChessAnalyzerApi.Middlewares;

public class LoggingMiddleware : IMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        #region заготовка для логгирования конкретнех полей
        #region requestModel
        ////HttpRequest request = context.Request;
        ////var requestDateTime = DateTime.Now;
        ////var traceId = context.TraceIdentifier;
        ////var ip = request.HttpContext.Connection.RemoteIpAddress;
        ////var clientIp = ip?.ToString();

        ////var requestMethod = request.Method;
        ////var requestPath = request.Path;
        ////var requestQuery = request.QueryString.ToString();
        ////var requestQueries = FormatQueries(request.QueryString.ToString());
        ////var requestHeaders = FormatHeaders(request.Headers);
        ////var requestBody = await ReadBodyFromRequest(request);
        ////var requestScheme = request.Scheme;
        ////var requestHost = request.Host.ToString();
        ////var requestContentType = request.ContentType;
        #endregion
        #region responseModel
        ////// Temporarily replace the HttpResponseStream, which is a write-only stream, with a MemoryStream to capture it's value in-flight.
        ////HttpResponse response = context.Response;
        ////var originalResponseBody = response.Body;
        ////using var newResponseBody = new MemoryStream();
        ////response.Body = newResponseBody;

        ////newResponseBody.Seek(0, SeekOrigin.Begin);
        ////var responseBodyText = await new StreamReader(response.Body).ReadToEndAsync();

        ////newResponseBody.Seek(0, SeekOrigin.Begin);
        ////await newResponseBody.CopyToAsync(originalResponseBody);

        ////var responseContentType = response.ContentType;
        ////var responseStatus = response.StatusCode.ToString();
        ////var responseHeaders = FormatHeaders(response.Headers);
        ////var responseBody = responseBodyText;
        ////var responseDateTimeUtc = DateTime.Now;
        #endregion
        #endregion

        // Логгирование начала обработки запроса
        _logger.LogInformation($"Request: {context.Request.Path} - {context.Request.Method}");

        try
        {
            await next(context); // вызов следующего middleware в цепочке
        }
        catch (Exception ex)
        {
            // Логгирование ошибки
            _logger.LogError(ex, $"An error occurred while processing the request: {context.Request.Path}");
            throw;
        }

        // Логгирование завершения обработки запроса
        _logger.LogInformation($"Request: {context.Request.Path} - {context.Request.Method} completed with status code: {context.Response.StatusCode}");
    }

    #region Методы для форматированного логгирования
    private Dictionary<string, string> FormatHeaders(IHeaderDictionary headers)
    {
        Dictionary<string, string> pairs = new();
        foreach (var header in headers)
        {
            pairs.Add(header.Key, header.Value);
        }
        return pairs;
    }

    private List<KeyValuePair<string, string>> FormatQueries(string queryString)
    {
        // переписать этот метод!!! 
        List<KeyValuePair<string, string>> pairs = new();
        string key, value;
        foreach (var query in queryString.TrimStart('?').Split("&"))
        {
            var items = query.Split("=");
            key = items.Length >= 1 ? items[0] : string.Empty;
            value = items.Length >= 2 ? items[1] : string.Empty;
            if (!string.IsNullOrEmpty(key))
            {
                pairs.Add(new KeyValuePair<string, string>(key, value));
            }
        }
        return pairs;
    }

    private async Task<string> ReadBodyFromRequest(HttpRequest request)
    {
        // Ensure the request's body can be read multiple times (for the next middlewares in the pipeline).
        request.EnableBuffering();
        using var streamReader = new StreamReader(request.Body, leaveOpen: true);
        var requestBody = await streamReader.ReadToEndAsync();
        // Reset the request's body stream position for next middleware in the pipeline.
        request.Body.Position = 0;
        return requestBody;
    }
    #endregion
}