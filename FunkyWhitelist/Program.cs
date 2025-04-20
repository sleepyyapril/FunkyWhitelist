using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FunkyWhitelist.Services;

namespace FunkyWhitelist;

public class Program
{
    private static IConfiguration _configuration = null!;
    private static IServiceProvider _services = null!;
    
    private const string ConfigurationFileName = "FunkyWhitelist.json";

    private static readonly DiscordSocketConfig SocketConfig = new()
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.GuildMembers,
        AlwaysDownloadUsers = true,
    };

    public static async Task Main(string[] args)
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile(ConfigurationFileName, false, false)
            .Build();
        
        var connectAddress = _configuration["connect_address"];
        var apiToken = _configuration["admin_api_token"];
        
        if (string.IsNullOrWhiteSpace(connectAddress) || string.IsNullOrWhiteSpace(apiToken))
        {
            Console.WriteLine("Please provide valid connect_address and/or api_token.");
            return;
        }
        
        _services = new ServiceCollection()
            .AddSingleton(_configuration)
            .AddSingleton(SocketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandlingService>()
            .AddScoped<WhitelistService>(_ => new WhitelistService(connectAddress, apiToken))
            .BuildServiceProvider();

        var commands = _services.GetRequiredService<CommandHandlingService>();
        await commands.InitializeAsync();

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