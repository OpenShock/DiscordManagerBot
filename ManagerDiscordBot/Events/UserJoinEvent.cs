using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace OpenShock.ManagerDiscordBot.Events;

public static class UserJoinEvent
{
    public const ulong WelcomeChannelId = 1224440439646457950;
    
    public static async Task ClientOnUserJoined(SocketGuildUser userJoin)
    {
        var general = userJoin.Guild.GetChannel(WelcomeChannelId) as SocketTextChannel;
        if (general == null)
        {
            OpenShockManagerBot.Logger.LogWarning("Could not find welcome channel");
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