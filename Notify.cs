using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace asdfasdf;

public static class Notify
{
    private static WebSocket ws;
    private static WebApplication app;
    private static HttpContext httpContext;
    private static CancellationToken cancellationToken;
    
    private const string json = "{}\x1e";

    public static void Init(WebApplication app)
    {
        app.MapPost("/hub/v1/negotiate", (HttpRequest request) =>
        {
            Guid guid = Guid.NewGuid();

            AvailableTransports transport = new()
            {
                transport = "WebSockets",
                transferFormats = ["Text"]
            };
    
            HubNego nego = new()
            {
                negotiateVersion = "0",
                connectionId = guid.ToString(),
                connectionToken = guid.ToString(),
                availableTransports = [transport]
            };
    
            return Results.Created("/hub/v1/negotiate", nego);
        });

        app.Map("/hub/v1", async (HttpContext context, CancellationToken cancellationToken) =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                await InitNotify(context, cancellationToken, app);
            }
        });
    }

    public static async Task InitNotify(HttpContext context, CancellationToken token, WebApplication wapp)
    {
        app = wapp;
        httpContext = context;
        cancellationToken = token;
        
        ws = await context.WebSockets.AcceptWebSocketAsync();
        
        await HandleConnection();
    }

    public static async Task HandleConnection()
    {
        while (!cancellationToken.IsCancellationRequested && ws.State == WebSocketState.Open)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.25f));
            
            var buffer = new byte[8];
            using var memoryStream = new MemoryStream();
            WebSocketReceiveResult result;

            do
            {
                result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                memoryStream.Write(buffer, 0, result.Count);
            } while (!result.EndOfMessage);

            buffer = memoryStream.ToArray();

            string receive = Encoding.UTF8.GetString(buffer).TrimEnd('\x1e');
            
            app.Logger.LogInformation(receive);

            app.Logger.LogInformation(receive.Length.ToString());
            
            if (receive.Length == 33)
            {
                app.Logger.LogInformation("sending");
                await ws.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (receive.Contains("SubscribeToPlayers"))
            {
                var response = new
                {
                    type = 3,
                    invocationId = "fuckyuo",
                    result = (object?)null
                };
                
                app.Logger.LogInformation("SubscribeToPlayers");
                // app.Logger.LogInformation(File.ReadAllText("websocket1.json") + "␞");
                // await ws.SendAsync(Encoding.UTF8.GetBytes(File.ReadAllText("websocket1.json") + "␞"), WebSocketMessageType.Text, true, CancellationToken.None);

                await ws.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response) + "\x1e"), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (receive.Length == 10)
            {
                app.Logger.LogInformation("type 6 ping or something idfk");

                await ws.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        
        if (ws.State == WebSocketState.CloseReceived)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
        }

        app.Logger.LogInformation("WebSocket closed");
    }
}