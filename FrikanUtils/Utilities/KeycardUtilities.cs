using System.Linq;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using MEC;
using KeycardItem = InventorySystem.Items.Keycards.KeycardItem;

namespace FrikanUtils.Utilities;

public static class KeycardUtilities
{
    /// <summary>
    /// Check whether a player has a correct keycard in their inventory for a permission requester (locker, doors, generators).
    /// </summary>
    /// <param name="requester">The permission requester, indicates which permissions are needed</param>
    /// <param name="player">Player to check permissions for</param>
    /// <param name="consume">Whether to consume the keycard if applicable</param>
    /// <returns>Whether the player has permissions</returns>
    public static bool HasKeycardPermission(this IDoorPermissionRequester requester, Player player,
        bool consume = false)
    {
        KeycardItem accessPassAvailable = null;
        foreach (var item in player.Items.Where(x => x != null))
        {
            if (item.Base is IDoorPermissionProvider provider &&
                requester.PermissionsPolicy.CheckPermissions(provider, requester, out _))
            {
                if (provider is KeycardItem keycard && keycard.ItemTypeId == ItemType.SurfaceAccessPass)
                {
                    accessPassAvailable = keycard;
                    continue;
                }

                return true;
            }
        }

        if (accessPassAvailable != null)
        {
            if (consume)
            {
                Timing.CallDelayed(.1f, () => { player.RemoveItem(accessPassAvailable); });
            }

            return true;
        }

        return requester.PermissionsPolicy.CheckPermissions(player.ReferenceHub, requester, out _);
    }
}