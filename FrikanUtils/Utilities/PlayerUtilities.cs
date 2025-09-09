using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace FrikanUtils.Utilities;

public static class PlayerUtilities
{
    private static readonly RoleTypeId[] BlacklistedRoles =
    [
        RoleTypeId.Filmmaker,
        RoleTypeId.Overwatch,
        RoleTypeId.Tutorial
    ];

    internal static readonly List<Player> BlacklistedPlayers = [];

    /// <summary>
    /// Get a list of all players excluding some blacklisted roles and the audio bot.
    /// This is intended for use during events.
    /// </summary>
    /// <returns>All players</returns>
    public static IEnumerable<Player> GetPlayers()
    {
        return Player.List.Where(player => !BlacklistedRoles.Contains(player.Role) &&
                                           !BlacklistedPlayers.Contains(player) &&
                                           !player.IsHost);
    }

    /// <summary>
    /// Teleport all players gotten from <see cref="GetPlayers"/> to a location as a certain role.
    /// Meant for spawning in a lot of players at once.
    ///
    /// Optionally, allows you to clear the inventory after spawning te player, however it does not give any items by default.
    /// </summary>
    /// <param name="role">New role for the player</param>
    /// <param name="location">Location to teleport to</param>
    /// <param name="clearInventory">Whether to clear the inventory</param>
    public static void TeleportAll(RoleTypeId role, Vector3 location, bool clearInventory = true)
    {
        foreach (var player in GetPlayers())
        {
            if (player.Role != role) player.SetRole(role, flags: RoleSpawnFlags.None);
            if (clearInventory) player.ClearInventory();
        }

        Timing.CallDelayed(0.1f, () =>
        {
            foreach (var player in GetPlayers())
            {
                player.Position = location;
            }
        });
    }

    /// <summary>
    /// Registers a player as an NPC, this will cause them to no longer be included the <see cref="GetPlayers"/>.
    /// </summary>
    /// <param name="npc">Player that is an NPC</param>
    public static void RegisterNpc(Player npc) => BlacklistedPlayers.AddIfNotContains(npc);

    /// <summary>
    /// Unregisters a player as an NPC, this will cause them to be included in the <see cref="GetPlayers"/> again.
    /// </summary>
    /// <param name="npc">Player that is no longer an NPC</param>
    public static void UnregisterNpc(Player npc) => BlacklistedPlayers.Remove(npc);
}