using Discord;
using Discord.Interactions;

namespace OpenShock.ManagerDiscordBot.Commands;

public sealed class WikiCommand : InteractionModuleBase
{
    [SlashCommand("wiki", "Links to wiki")]
    public async Task Execute()
    {
        var embed = new EmbedBuilder().WithDescription("https://wiki.openshock.org/").WithColor(Color.Blue).WithTitle("Wiki").Build();
        
        await RespondAsync(embed: embed);
    }
}