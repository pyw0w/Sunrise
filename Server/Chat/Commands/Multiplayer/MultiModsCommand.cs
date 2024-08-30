using HOPEless.osu;
using osu.Shared;
using Sunrise.Server.Attributes;
using Sunrise.Server.Objects;
using Sunrise.Server.Types.Enums;
using Sunrise.Server.Types.Interfaces;

namespace Sunrise.Server.Chat.Commands.Multiplayer;

[ChatCommand("mods", "mp", isGlobal: true)]
public class MultiModsCommand : IChatCommand
{
    public Task Handle(Session session, ChatChannel? channel, string[]? args)
    {
        if (channel == null || session.Match == null)
        {
            throw new InvalidOperationException("Multiplayer command was called without being in a multiplayer room.");
        }

        if (session.Match.HasHostPrivileges(session) == false)
        {
            session.SendChannelMessage(channel.Name, "This command can only be used by the host of the room.");
            return Task.CompletedTask;
        }

        if (args == null || args.Length == 0)
        {
            session.SendChannelMessage(channel.Name, "Usage: !mp set <mod> [<mod>] [<mod>]...");
            return Task.CompletedTask;
        }

        var mods = new List<Mods>();
        var freeMods = false;

        foreach (var mod in args.Where(x => string.IsNullOrWhiteSpace(x) == false))
        {
            if (Enum.TryParse(mod, true, out ModsShorted modShortedEnum) == false)
            {
                if (mod.Equals("Freemod", StringComparison.CurrentCultureIgnoreCase))
                {
                    freeMods = true;
                    continue;
                }

                session.SendChannelMessage(channel.Name, $"Invalid mod: {mod}");
                return Task.CompletedTask;
            }

            var modEnum = (Mods)modShortedEnum;
            mods.Add(modEnum);
        }

        var currentMatch = session.Match.Match;

        currentMatch.SpecialModes = freeMods ? MultiSpecialModes.FreeMod : MultiSpecialModes.None;
        currentMatch.ActiveMods = mods.Aggregate((a, b) => a | b);

        session.Match.UpdateMatchSettings(currentMatch, session);

        return Task.CompletedTask;
    }
}