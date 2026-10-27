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
        public static string BaseURL = "http://100.126.252.79:2059"; // replace with your url
        public static int GameVersion = 20230406;
    }
}
