using System.Text;

namespace asdfasdf;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    
    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // First, store the original body stream
        var originalRequestBody = context.Request.Body;
        var originalResponseBody = context.Response.Body;

        try
        {
            // Read and log the request body
            string requestBody = await ReadRequestBodyAsync(context);
            LogRequest(context, requestBody);
            
            // Create a new memory stream for the response
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;
            
            // Continue the pipeline
            await _next(context);
            
            // Read and log the response body
            string responseBody = await ReadResponseBodyAsync(context.Response);
            LogResponse(context, responseBody);
            
            // Copy the response to the original stream and restore it
            await responseBodyStream.CopyToAsync(originalResponseBody);
        }
        finally
        {
            // Always restore the original streams
            context.Request.Body = originalRequestBody;
            context.Response.Body = originalResponseBody;
        }
    }
    
    private async Task<string> ReadRequestBodyAsync(HttpContext context)
    {
        // We need to read the request body and then rewind it
        context.Request.EnableBuffering();
        
        using var streamReader = new StreamReader(
            context.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);
        
        var requestBody = await streamReader.ReadToEndAsync();
        
        // IMPORTANT: Reset the position to 0 to allow other middleware to read it
        context.Request.Body.Position = 0;
        
        return requestBody;
    }
    
    private async Task<string> ReadResponseBodyAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        
        return responseBody;
    }
      private void LogRequest(HttpContext context, string requestBody)
    {
        _logger.LogInformation(
            "HTTP {Method} {Path} received at {Time}\n" +
            "Request Headers: {Headers}\n" +
            "Request Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            DateTime.UtcNow,
            FormatHeaders(context.Request.Headers),
            requestBody);
    }
    
    private void LogResponse(HttpContext context, string responseBody)
    {
        _logger.LogInformation(
            "HTTP {StatusCode} returned for {Method} {Path}\n" +
            "Response Headers: {Headers}\n" +
            "Response Body: {Body}",
            context.Response.StatusCode,
            context.Request.Method,
            context.Request.Path,
            FormatHeaders(context.Response.Headers),
            responseBody);
    }
    
    private string FormatHeaders(IHeaderDictionary headers)
    {
        var sb = new StringBuilder();
        foreach (var (key, value) in headers)
        {
            sb.AppendLine($"{key}: {string.Join(", ", value)}");
        }
        return sb.ToString();
    }
}

public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}