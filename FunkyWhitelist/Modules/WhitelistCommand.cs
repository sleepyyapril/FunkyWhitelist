using Discord.Interactions;
using FunkyWhitelist.Services;

namespace FunkyWhitelist.Modules;

public class WhitelistCommand : InteractionModuleBase<SocketInteractionContext>
{
    public WhitelistService WhitelistService { get; set; }

    [SlashCommand("whitelist", "Whitelists a user based on name or on the message you're replying to's author.")]
    public async Task Whitelist(string name)
    {
        var response = await WhitelistService.WhitelistUser(name);
        await RespondAsync(response, ephemeral: true);
    }
}