namespace MerchStore.Middleware;

public class SessionLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SessionLoggingMiddleware> _logger;

    public SessionLoggingMiddleware(RequestDelegate next, ILogger<SessionLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Ensure session is available
        if (context.Session != null)
        {
            // Get or create session ID
            var sessionId = context.Session.Id;

            // Get or set session start time
            const string sessionStartKey = "SessionStartTime";
            if (!context.Session.Keys.Contains(sessionStartKey))
            {
                context.Session.SetString(sessionStartKey, DateTime.UtcNow.ToString("o"));
                _logger.LogInformation("New session started. SessionId: {SessionId}", sessionId);
            }

            var sessionStartTime = context.Session.GetString(sessionStartKey);
            var sessionDuration = DateTime.UtcNow - DateTime.Parse(sessionStartTime!);

            // Create session scope
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["SessionId"] = sessionId,
                ["SessionDuration"] = sessionDuration.TotalMinutes,
                ["PageViewsInSession"] = context.Session.GetInt32("PageViews") ?? 0,
                ["User"] = context.User.Identity?.Name ?? "Anonymous"
            }))
            {
                // Increment page view counter
                var pageViews = context.Session.GetInt32("PageViews") ?? 0;
                context.Session.SetInt32("PageViews", pageViews + 1);

                // Log the page view
                _logger.LogInformation("Page view: {Path} - View #{PageView} in session",
                    context.Request.Path, pageViews + 1);

                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }
    }
}

public static class SessionLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseSessionLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SessionLoggingMiddleware>();
    }
}