using LabApi.Features.Wrappers;
using PlayerRoles;

namespace FrikanUtils.Utilities;

/// <summary>
/// Couple of extensions to help determine whether someone is a main SCP.
/// </summary>
public static class ScpExtension
{
    /// <summary>
    /// Check whether the player is an SCP, but not SCP-049-2.
    /// </summary>
    /// <param name="player">Player to check</param>
    /// <returns>Whether the player is a main SCP</returns>
    public static bool IsMainScp(this Player player)
    {
        return player.Team == Team.SCPs && player.Role != RoleTypeId.Scp0492;
    }

    /// <summary>
    /// Check whether the role is an SCP, but not SCP-049-2.
    /// </summary>
    /// <param name="role">Role to check</param>
    /// <returns>Whether the player is a main SCP</returns>
    public static bool IsMainScp(this RoleTypeId role)
    {
        return role.GetTeam() == Team.SCPs && role != RoleTypeId.Scp0492;
    }
}