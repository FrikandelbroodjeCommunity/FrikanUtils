using System.Collections.Generic;
using System.Linq;
using FrikanUtils.Npc;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace FrikanUtils.Utilities;

public static class PlayerUtilities
{
    public static IEnumerable<RoleTypeId> BlacklistedRoles => BlacklistedRolesInternal;

    private static readonly RoleTypeId[] BlacklistedRolesInternal =
    [
        RoleTypeId.Filmmaker,
        RoleTypeId.Overwatch,
        RoleTypeId.Tutorial
    ];
    
    /// <summary>
    /// Get a list of all players excluding some blacklisted roles and the audio bot.
    /// This is intended for use during events.
    /// </summary>
    /// <returns>All players</returns>
    public static IEnumerable<Player> GetPlayers()
    {
        return Player.List.Where(player => !BlacklistedRoles.Contains(player.Role) &&
                                           !NpcSystem.Npcs.Contains(player) &&
                                           !player.IsHost &&
                                           player.UserId != null).ToArray();
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
}