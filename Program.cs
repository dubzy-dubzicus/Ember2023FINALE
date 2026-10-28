using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Ember2023;
using Ember2023.Classes;
using Ember2023.Classes.DBs;
using Ember2023.Classes.DBs.DBClasses;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Ember2023
{
    public class Program
    {
        public static string dataDir = Path.Combine(Environment.CurrentDirectory, "Data");

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (!Directory.Exists(dataDir))
            {
                Console.WriteLine($"Setting up data directory...");

                string[] foldersToCreate = new[]
                {
                    dataDir,
                    Path.Combine(dataDir, "Images"),
                    Path.Combine(dataDir, "Images", "PlayerImages"),
                    Path.Combine(dataDir, "Images", "PolaroidImages"),
                    Path.Combine(dataDir, "CDN", "DataBlobs"),
                    Path.Combine(dataDir, "CDN", "InventionBlobs"),
                    Path.Combine(dataDir, "CDN", "RoomBlobs"),
                    Path.Combine(dataDir, "Imports"),
                };

                foreach (var folder in foldersToCreate)
                {
                    Directory.CreateDirectory(folder);
                }

                Console.WriteLine($"Set up Data directory at {dataDir}");
            }

            builder.Services.AddControllers().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            builder.Services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.None;
            });

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxConcurrentConnections = 500;
                options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
                options.Limits.MinResponseDataRate = null;
                options.Limits.MinRequestBodyDataRate = null;
            });

            var app = builder.Build();
            
            app.UseHttpLogging();
            
            app.UseWebSockets();
            
            app.UseRouting();
            
            app.UseAuthorization();

            app.MapControllers();
            
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Program.dataDir, "CDN", "room")),
                RequestPath = "/cdn/room",
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            });

            // Load and register loading screen URLs
            string rewrittenLoadscreenJson = null;
            try
            {
                var loadingscreenPath = Path.Combine(Program.dataDir, "Imports", "loadingscreen.json");
                if (System.IO.File.Exists(loadingscreenPath))
                {
                    var loadingscreenJson = await System.IO.File.ReadAllTextAsync(loadingscreenPath);
                    rewrittenLoadscreenJson = Classes.LoadingScreenImageService.RewriteConfiguration(loadingscreenJson);
                    var rewrittenPath = Path.Combine(Program.dataDir, "Imports", "loadingscreen.rewritten.json");
                    await System.IO.File.WriteAllTextAsync(rewrittenPath, rewrittenLoadscreenJson);
                    Console.WriteLine("[LoadingScreenImageService] Registered loading screen URLs and saved rewritten config");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadingScreenImageService] Failed to load config: {ex.Message}");
            }

            // Mark all RRO rooms as cloneable/templates
            try
            {
                RoomDB.MarkRROsAsCloneable();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RoomDB] Failed to mark RROs as cloneable: {ex.Message}");
            }
                        
            Task.Run(() => Cloudflare.StartCloudflared());
            
            app.Run("http://localhost:2059");
        }
    }
}
