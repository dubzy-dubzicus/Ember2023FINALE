using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Ember2023
{
    public class Cloudflare
    {
        public static void StartCloudflared()
        {
            // Local deployment uses Tailscale Funnel, not a cloud/VPS tunnel.
            // The legacy Linux cloudflared path is intentionally not started.
            Console.WriteLine("[Cloudflared] Skipped; using Tailscale Funnel for https://ember.tail298318.ts.net.");
        }
    }
}