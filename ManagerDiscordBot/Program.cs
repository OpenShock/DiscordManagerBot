using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenShock.ManagerDiscordBot;
using OpenShock.ManagerDiscordBot.Events;
using OpenShock.ManagerDiscordBot.Services;
using OpenShock.ManagerDiscordBot.Utils;
using Serilog;
using System.Reflection;

HostBuilder builder = new();

builder.UseContentRoot(Directory.GetCurrentDirectory())
    .ConfigureHostConfiguration(config =>
    {
        config.AddEnvironmentVariables(prefix: "DOTNET_");
        if (args is { Length: > 0 }) config.AddCommandLine(args);
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true,
                reloadOnChange: false);

        config.AddEnvironmentVariables();
        if (args is { Length: > 0 }) config.AddCommandLine(args);
    })
    .UseDefaultServiceProvider((context, options) =>
    {
        var isDevelopment = context.HostingEnvironment.IsDevelopment();
        options.ValidateScopes = isDevelopment;
        options.ValidateOnBuild = isDevelopment;
    })
    .UseSerilog((context, _, config) => { config.ReadFrom.Configuration(context.Configuration); });

// <---- Services ---->

builder.ConfigureServices(services =>
{
    services.AddSingleton<ManagerBotConfig>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        return configuration.GetSection("bot").Get<ManagerBotConfig>()
               ?? throw new Exception("Could not load bot config");
    });


    services.AddSingleton<DiscordSocketClient>(new DiscordSocketClient(new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers |
                         GatewayIntents.MessageContent
    }));
    services.AddSingleton<InteractionService>();
    services.AddSingleton<InteractionHandler>();
});

try
{
    var host = builder.Build();
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Starting OpenShock Manager Discord Bot version {Version}",
        Assembly.GetEntryAssembly()?.GetName().Version?.ToString());

    // <---- Initialize Service stuff, this also instantiates the singletons!!! ---->

    var client = host.Services.GetRequiredService<DiscordSocketClient>();
    var interactionService = host.Services.GetRequiredService<InteractionService>();
    var interactionHandler = host.Services.GetRequiredService<InteractionHandler>();

    client.Log += LoggingUtils.LogAsync;
    client.Ready += () => ReadyAsync(client, interactionService);

    interactionService.Log += LoggingUtils.LogAsync;

    await interactionHandler.InitializeAsync();

    // <---- Run discord client ---->

    var config = host.Services.GetRequiredService<ManagerBotConfig>();

    client.UserJoined += UserJoinEvent.ClientOnUserJoined;
    await client.LoginAsync(TokenType.Bot, config.Token);
    await client.StartAsync();

    host.Run();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

async Task ReadyAsync(BaseSocketClient client, InteractionService interactionService)
{
    Log.Information("Connected as [{CurrentUser}]", client.CurrentUser.Username);
    await client.SetActivityAsync(
        new Game($"electricity flow, v{Assembly.GetEntryAssembly()?.GetName().Version?.ToString()}", ActivityType.Watching));

    await interactionService.RegisterCommandsToGuildAsync(Constants.GuildId);
}