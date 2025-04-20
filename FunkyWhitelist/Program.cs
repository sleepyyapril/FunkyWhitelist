using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FunkyWhitelist.Services;

namespace FunkyWhitelist;

public class Program
{
    private static IConfiguration _configuration = null!;
    private static IServiceProvider _services = null!;
    
    private const string ConfigurationFileName = "WhitelistBot.json";

    private static readonly DiscordSocketConfig SocketConfig = new()
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.MessageContent,
        AlwaysDownloadUsers = true,
    };

    public static async Task Main(string[] args)
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile(ConfigurationFileName, false, false)
            .Build();
        
        var connectAddress = _configuration["connect_address"];
        var apiToken = _configuration["admin_api_token"];
        
        _services = new ServiceCollection()
            .AddSingleton(_configuration)
            .AddSingleton(SocketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()
            .AddSingleton(x => new WhitelistService(connectAddress, apiToken))
            .BuildServiceProvider();

        var client = _services.GetRequiredService<DiscordSocketClient>();
        client.Log += LogAsync;
        
        await client.LoginAsync(TokenType.Bot, _configuration["token"]);
        await client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private static Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message.ToString());
        return Task.CompletedTask;
    }
}