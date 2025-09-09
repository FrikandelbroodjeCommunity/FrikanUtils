using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;

namespace FrikanUtils.CustomDummyActions;

internal struct ActionExecutable
{
    public Func<Player, List<Player>, string> Action;
    public List<Player> Players;

    public bool IsValid() => Players is { Count: > 0 } && Action != null;

    internal string Invoke(Player player)
    {
        return Action.Invoke(player, Players);
    }
}