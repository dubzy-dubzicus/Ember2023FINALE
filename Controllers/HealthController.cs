using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Ember2023.Controllers
{
    [ApiController]
    [Route("/api/health")]
    public class HealthController : ControllerBase
    {
        private static readonly DateTime StartTime = DateTime.UtcNow;
        private static readonly Dictionary<string, long> RequestCounts = new();
        private static readonly object CountLock = new();

        public static void IncrementRequestCount(string path)
        {
            lock (CountLock)
            {
                if (RequestCounts.ContainsKey(path))
                    RequestCounts[path]++;
                else
                    RequestCounts[path] = 1;
            }
        }

        [HttpGet("v1/status")]
        public IActionResult GetStatus()
        {
            var uptime = DateTime.UtcNow - StartTime;
            var process = Process.GetCurrentProcess();

            lock (CountLock)
            {
                var totalRequests = RequestCounts.Values.Sum();

                return Ok(new
                {
                    status = "healthy",
                    uptimeSeconds = uptime.TotalSeconds,
                    uptimeFormatted = FormatUptime(uptime),
                    memoryMB = process.PrivateMemorySize64 / 1024 / 1024,
                    cpuTotalProcessorTime = process.TotalProcessorTime.TotalSeconds,
                    threadCount = process.Threads.Count,
                    totalRequests = totalRequests,
                    requestsByEndpoint = RequestCounts.ToDictionary(k => k.Key, v => v.Value),
                    serverTime = DateTime.UtcNow.ToString("O"),
                    version = "1.0.0"
                });
            }
        }

        [HttpGet("v1/ping")]
        public IActionResult Ping()
        {
            return Ok(new { pong = true, time = DateTime.UtcNow.ToString("O") });
        }

        [HttpGet("v1/metrics")]
        public IActionResult GetMetrics()
        {
            var process = Process.GetCurrentProcess();
            var uptime = DateTime.UtcNow - StartTime;

            return Ok(new
            {
                uptime_seconds = uptime.TotalSeconds,
                memory_used_mb = process.PrivateMemorySize64 / 1024 / 1024,
                memory_paged_mb = process.PagedMemorySize64 / 1024 / 1024,
                memory_virtual_mb = process.VirtualMemorySize64 / 1024 / 1024,
                cpu_time_seconds = process.TotalProcessorTime.TotalSeconds,
                thread_count = process.Threads.Count,
                handle_count = process.HandleCount,
                server_time_utc = DateTime.UtcNow.ToString("O")
            });
        }

        private static string FormatUptime(TimeSpan ts)
        {
            if (ts.TotalDays >= 1)
                return $"{(int)ts.TotalDays}d {ts.Hours}h {ts.Minutes}m {ts.Seconds}s";
            if (ts.TotalHours >= 1)
                return $"{ts.Hours}h {ts.Minutes}m {ts.Seconds}s";
            if (ts.TotalMinutes >= 1)
                return $"{ts.Minutes}m {ts.Seconds}s";
            return $"{ts.Seconds}s";
        }
    }

    [ApiController]
    public class RateLimitController : ControllerBase
    {
        private static readonly Dictionary<string, RateLimitEntry> ClientLimits = new();
        private static readonly object LimitLock = new();

        private const int MaxRequestsPerMinute = 600; // 10 per second per IP
        private const int MaxRequestsPerHour = 36000;

        [HttpGet("v1/rate-limit/status")]
        public IActionResult GetRateLimitStatus()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            lock (LimitLock)
            {
                if (ClientLimits.TryGetValue(ip, out var entry))
                {
                    return Ok(new
                    {
                        ip = ip,
                        requestsLastMinute = entry.RequestsLastMinute.Count,
                        requestsLastHour = entry.RequestsLastHour.Count,
                        maxPerMinute = MaxRequestsPerMinute,
                        maxPerHour = MaxRequestsPerHour,
                        remainingLastMinute = Math.Max(0, MaxRequestsPerMinute - entry.RequestsLastMinute.Count),
                        remainingLastHour = Math.Max(0, MaxRequestsPerHour - entry.RequestsLastHour.Count)
                    });
                }
            }

            return Ok(new
            {
                ip = ip,
                requestsLastMinute = 0,
                requestsLastHour = 0,
                maxPerMinute = MaxRequestsPerMinute,
                maxPerHour = MaxRequestsPerHour,
                remainingLastMinute = MaxRequestsPerMinute,
                remainingLastHour = MaxRequestsPerHour
            });
        }

        public class RateLimitEntry
        {
            public List<DateTime> RequestsLastMinute { get; set; } = new();
            public List<DateTime> RequestsLastHour { get; set; } = new();
        }
    }
}
