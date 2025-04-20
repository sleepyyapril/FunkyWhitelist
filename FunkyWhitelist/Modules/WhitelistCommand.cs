using Discord;
using Discord.Commands;
using FunkyWhitelist.Services;

namespace FunkyWhitelist.Modules;

public class WhitelistCommand : ModuleBase<SocketCommandContext>
{
    public WhitelistService WhitelistService { get; set; } = null!;
    
    private const ulong WhitelistChannelId = 1295833135992537130;

    [Command("whitelist")]
    [RequireContext(ContextType.Guild)]
    [RequireRole(1297993874576244788)]
    public async Task Whitelist()
    {
        if (Context.Message.ReferencedMessage is not { } reply
            || Context.Message.Channel.Id != WhitelistChannelId)
            return;
        
        var username = reply.Content;
        var response = await WhitelistService.WhitelistUser(username);
        
        await Context.Message.DeleteAsync();

        if (response == "UnprocessableContent")
        {
            await reply.ReplyAsync($"Username ``{username}`` was not found.");
            return;
        }

        if (response != null)
        {
            await ReplyAsync(response);
            return;
        }
        
        if (!Emote.TryParse("<:fs_funk:1287607018550595688>", out var funkyEmote))
            return;
        
        await reply.AddReactionAsync(funkyEmote);
    }
}