using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using asdfasdf;
using asdfasdf.APIs;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate("x509", options =>
{
    options.RevocationMode = X509RevocationMode.NoCheck;
});

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(443, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        listenOptions.UseHttps();
    });

    
    options.ConfigureHttpsDefaults(h =>
    {
        h.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
        h.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
    });
});

builder.Services.AddAuthorization();

builder.Services.AddHttpLogging();
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.UseRequestResponseLogging();
}

WebSocketOptions wsOptions = new()
{
    KeepAliveInterval = TimeSpan.FromMinutes(1)
};

app.UseHttpLogging();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets(wsOptions);

CreateFolder("ServerData");
CreateFolder("ServerData/Accounts");
CreateFolder("ServerData/CachedAccounts");
CreateFolder("ServerData/BugReports");
CreateFolder("ServerData/Rooms");

CreateFolder("ServerData/Accounts/1");
CreateFolder("ServerData/Accounts/2");

if (!File.Exists("ServerData/Accounts/1/user.json"))
{
    BulkAccount tAccount = new()
    {
        AvailableUsernameChanges = 1,
        Email = "coach@coach.com",
        Birthday = DateTime.MinValue,
        IsFakeJuniorBirthday = false,
        accountId = 1,
        username = "Coach",
        displayName = "Coach",
        displayEmoji = "",
        profileImage = "DefaultProfileImage.png",
        bannerImage = "DefaultProfileImage.png",
        isJunior = false,
        platforms = PlatformType.All,
        personalPronouns = PersonalPronouns.None,
        identityFlags = IdentityFlags.None,
        createdAt = DateTime.UtcNow,
        isMetaPlatformBlocked = false
    };
    
    File.WriteAllText("ServerData/Accounts/1/user.json", JsonSerializer.Serialize(tAccount));
}

if (!File.Exists("ServerData/Accounts/2/user.json"))
{
    BulkAccount tAccount = new()
    {
        AvailableUsernameChanges = 1,
        Email = "email@email.com",
        Birthday = DateTime.MinValue,
        IsFakeJuniorBirthday = false,
        accountId = 0,
        username = Environment.UserName,
        displayName = Environment.UserName,
        displayEmoji = "",
        profileImage = "DefaultProfileImage.png",
        bannerImage = "DefaultProfileImage.png",
        isJunior = false,
        platforms = PlatformType.All,
        personalPronouns = PersonalPronouns.None,
        identityFlags = IdentityFlags.None,
        createdAt = DateTime.UtcNow,
        isMetaPlatformBlocked = false
    };
    
    File.WriteAllText("ServerData/Accounts/2/user.json", JsonSerializer.Serialize(tAccount));
}

if (!File.Exists("ServerData/Accounts/2/PlayerSettings.json"))
{
    File.Copy("DefaultPlayerSettings.json", "ServerData/Accounts/2/PlayerSettings.json");
}

if (!File.Exists("ServerData/Accounts/2/avoidjuniors"))
{
    File.WriteAllText("ServerData/Accounts/2/avoidjuniors", "false");
}

if (!File.Exists("ServerData/Accounts/2/Avatar.json"))
{
    File.Copy("DefaultAvatar.json", "ServerData/Accounts/2/Avatar.json");
}

if (!File.Exists("ServerData/Config.json"))
{
    ServerConfig.config = new()
    {
        PhotonRegion = string.Empty,
        PhotonInstanceId = string.Empty
    };
    
    File.WriteAllText("ServerData/Config.json", JsonSerializer.Serialize(ServerConfig.config));
}
else
{
    ServerConfig.config = JsonSerializer.Deserialize<ServerConfig>(File.ReadAllText("ServerData/Config.json"));
}

NameServer.Init(app);
API.Init(app);
Auth.Init(app);
Accounts.Init(app);
Matchmaking.Init(app);
Commerce.Init(app);
Moderation.Init(app);
Studio.Init(app);
Notify.Init(app);
Rooms.Init(app);
Data.Init(app);
CDN.Init(app);
DataCollection.Init(app);
Econ.Init(app);
PlayerSettingsAPI.Init(app);
Clubs.Init(app);
AI.Init(app);
PlatformNotifications.Init(app);
Chat.Init(app);
BugReporting.Init(app);
Storage.Init(app);
Images.Init(app);
Amplitude.Init(app);

app.MapGet("/generate_204", Results.NoContent);

app.Run();


static void CreateFolder(string dir)
{
    if (!Directory.Exists(dir))
    {
        Directory.CreateDirectory(dir);
    }
}