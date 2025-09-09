using PlayerRoles;

namespace FrikanUtils.Utilities;

public static class ColorUtilities
{
    /// <summary>
    /// Get the hex code belonging to a role.
    /// </summary>
    /// <param name="role">The role to get the code for</param>
    /// <returns>Hex code for the role</returns>
    public static string GetHex(this RoleTypeId role)
    {
        return PlayerRoleLoader.AllRoles.TryGetValue(role, out var loadedRole)
            ? Misc.ToHex(loadedRole.RoleColor)
            : "#FFFFFF";
    }
}