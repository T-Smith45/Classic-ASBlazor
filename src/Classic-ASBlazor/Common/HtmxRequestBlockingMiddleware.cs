namespace Simple_Razor_Components.Common;

/// <summary>
/// Middleware to block requests that do not originate from HTMX,
/// specifically for paths starting with a configurable base path.
/// This blocking is only active when the application is NOT in the Development environment.
/// </summary>
public class HtmxRequestBlockingMiddleware
{
    private readonly RequestDelegate _next;
        private readonly ILogger<HtmxRequestBlockingMiddleware> _logger;
        private readonly string _basePath;
        private readonly IHostEnvironment _env; // Added to check environment

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmxRequestBlockingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger for the middleware.</param>
        /// <param name="env">The hosting environment, used to check if the application is in development mode.</param>
        /// <param name="basePath">The base path for which HTMX requests are enforced (e.g., "/htmx-only/").</param>
        public HtmxRequestBlockingMiddleware(RequestDelegate next, ILogger<HtmxRequestBlockingMiddleware> logger, IHostEnvironment env, string basePath)
        {
            _next = next;
            _logger = logger;
            _env = env; // Store the environment
            _basePath = basePath;
        }

        /// <summary>
        /// Invokes the middleware to process the HTTP request.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply the blocking logic if the application is NOT in the Development environment
            //if (!_env.IsDevelopment())
            //{
                // Check if the request path starts with the configured base path
                if (!_env.IsDevelopment() && context.Request.Path.StartsWithSegments(_basePath, System.StringComparison.OrdinalIgnoreCase))
                {
                    // Check for the 'HX-Request' header, which HTMX sends with a value of 'true'
                    if (!context.Request.Headers.TryGetValue("HX-Request", out var hxRequestHeader) ||
                        hxRequestHeader.ToString().ToLowerInvariant() != "true")
                    {
                        _logger.LogWarning("Blocking non-HTMX request to '{Path}'. Missing or invalid 'HX-Request' header. (Environment: {EnvironmentName})", context.Request.Path, _env.EnvironmentName);
                        context.Response.StatusCode = StatusCodes.Status403Forbidden; // Forbidden
                        return; // Stop processing the request
                    }
                }
            //}
            // else
            // {
            //     _logger.LogInformation("HTMX request blocking is bypassed in Development environment for path: '{Path}'.", context.Request.Path);
            // }

            // If the path doesn't match the base path, or it's an HTMX request, or in Development, proceed to the next middleware
            await _next(context);
        }
}

/// <summary>
/// Extension methods for adding the <see cref="HtmxRequestBlockingMiddleware"/> to the application's request pipeline.
/// </summary>
public static class HtmxRequestBlockingMiddlewareExtensions
{
    /// <summary>
    /// Adds the <see cref="HtmxRequestBlockingMiddleware"/> to the <see cref="IApplicationBuilder"/>'s pipeline.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance.</param>
    /// <param name="basePath">The base path for which HTMX requests are enforced (e.g., "/htmx-only/").</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder UseHtmxRequestBlocking(this IApplicationBuilder builder, string basePath)
    {
        // Use the middleware, passing the basePath as an argument to its constructor.
        // IHostEnvironment is automatically injected by ASP.NET Core's DI system.
        return builder.UseMiddleware<HtmxRequestBlockingMiddleware>(basePath);
    }
}