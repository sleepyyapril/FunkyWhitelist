using Discord;
using Discord.Commands;
using FunkyWhitelist.Services;

namespace FunkyWhitelist.Modules;

public class WhitelistCommand : ModuleBase<SocketCommandContext>
{
    public WhitelistService WhitelistService { get; set; } = null!;
    
    private const ulong WhitelistChannelId = 1295833135992537130;

    [Command("whitelist")]
    [Alias("wl")]
    [RequireContext(ContextType.Guild)]
    [RequireRole(1297993874576244788)]
    public async Task ReplyWhitelist(string username)
    {
        var response = await WhitelistService.WhitelistUser(username);
        
        await Context.Message.DeleteAsync();

        if (response == "UnprocessableEntity")
        {
            await ReplyAsync($":x: ``{username}`` is not a valid SS14 account.");
            return;
        }

        if (response == "Conflict")
        {
            await ReplyAsync($":x: {username} is already whitelisted.");
            return;
        }

        if (response != null)
            await ReplyAsync(response);
    }
    
    [Command("replywhitelist")]
    [Alias("replywl")]
    [RequireContext(ContextType.Guild)]
    [RequireRole(1297993874576244788)]
    public async Task ReplyWhitelist()
    {
        if (Context.Message.ReferencedMessage is not { } reply
            || Context.Message.Channel.Id != WhitelistChannelId)
            return;
        
        var username = reply.Content;
        var response = await WhitelistService.WhitelistUser(username);
        
        await Context.Message.DeleteAsync();

        if (response == "UnprocessableEntity")
        {
            await reply.ReplyAsync($":x: ``{username}`` is not a valid SS14 account.");
            return;
        }
        
        if (response == "Conflict")
        {
            await ReplyAsync($":x: {username} is already whitelisted.");
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
    
    [Command("unwhitelist")]
    [Alias("unwl")]
    [RequireContext(ContextType.Guild)]
    [RequireRole(1297993874576244788)]
    public async Task Unwhitelist(string username)
    {
        var response = await WhitelistService.UnwhitelistUser(username);
        
        await Context.Message.DeleteAsync();

        if (response == "UnprocessableEntity")
        {
            await ReplyAsync($":x: ``{username}`` is not a valid SS14 account.");
            return;
        }

        if (response == "NotFound")
        {
            await ReplyAsync($":x: {username} is not whitelisted.");
            return;
        }

        if (response != null)
            await ReplyAsync(response);
    }
}