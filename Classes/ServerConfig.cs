using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ember2023.Classes
{
    public class ServerConfig
    {
        public static object Bracket => new List<object>(); 
        // Self-hosted through Tailscale Funnel; override with EMBER_BASE_URL if needed.
        public static string BaseURL = Environment.GetEnvironmentVariable("EMBER_BASE_URL")
            ?? "https://ember.tail298318.ts.net";
        public static int GameVersion = 20230406;
    }
}
