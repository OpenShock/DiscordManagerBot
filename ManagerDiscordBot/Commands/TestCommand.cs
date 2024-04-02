using Discord.Interactions;

namespace OpenShock.ManagerDiscordBot.Commands;

public sealed class TestCommand : InteractionModuleBase
{
    [SlashCommand("test", "test")]
    public async Task Execute()
    {
        
    }
}