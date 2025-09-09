using System;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;

namespace FrikanUtils.Utilities;

public static class PermissionUtilities
{
    /// <summary>
    /// Safe way of getting permissions. Ensures no error will occur due to a player not being authenticated yet.
    /// </summary>
    /// <param name="player">Player to check the permissions for</param>
    /// <param name="permissions">Permissions to check for</param>
    /// <returns>Whether the player has the permissions</returns>
    public static bool SafeHasPermissions(this Player player, params string[] permissions)
    {
        try
        {
            return player?.UserId != null && player.HasPermissions(permissions);
        }
        catch (Exception)
        {
            return false;
        }
    }
}