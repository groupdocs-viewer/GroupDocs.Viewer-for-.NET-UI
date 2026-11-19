using System.Diagnostics;
using System.Globalization;

namespace GroupDocs.Viewer.UI.Sample.Performance;

public static class ServerTimingExtensions
{
    /// <summary>
    /// Measures the requests under <paramref name="path"/>: logs how long each one took and sends
    /// the time in the <c>Server-Timing</c> header, which the browser's developer tools show
    /// under Network &gt; Timing.
    /// </summary>
    public static WebApplication UseServerTiming(this WebApplication app, PathString path)
    {
        app.Use(async (context, next) =>
        {
            if (!context.Request.Path.StartsWithSegments(path))
            {
                await next(context);
                return;
            }

            var started = Stopwatch.GetTimestamp();
            context.Response.OnStarting(() =>
            {
                var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
                context.Response.Headers.Append("Server-Timing",
                    $"app;dur={elapsed.ToString("0.0", CultureInfo.InvariantCulture)}");
                return Task.CompletedTask;
            });

            await next(context);

            app.Logger.LogInformation("{Method} {Path}{Query} {StatusCode} in {Elapsed:0} ms",
                context.Request.Method, context.Request.Path, context.Request.QueryString,
                context.Response.StatusCode, Stopwatch.GetElapsedTime(started).TotalMilliseconds);
        });

        return app;
    }
}
