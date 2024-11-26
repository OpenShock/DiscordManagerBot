using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Serilog;

namespace OpenShock.ManagerDiscordBot.Events;

public static class UserJoinEvent
{
    public static async Task ClientOnUserJoined(SocketGuildUser userJoin)
    {
        var general = userJoin.Guild.GetChannel(Constants.WelcomeChannelId) as SocketTextChannel;
        if (general == null)
        {
            Log.Warning("Could not find welcome channel");
            return;
        }
        
        var embed = new EmbedBuilder().WithAuthor(userJoin.Username, userJoin.GetAvatarUrl())
            .WithDescription($"Welcome to the server, {userJoin.Mention}!")
            .WithColor(14764653)
            .WithCurrentTimestamp()
            .Build();


        await general.SendMessageAsync(embed: embed);
    }
}