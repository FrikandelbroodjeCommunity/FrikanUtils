using InventorySystem.Items.Usables.Scp1344;
using LabApi.Features.Wrappers;
using Scp1344Item = LabApi.Features.Wrappers.Scp1344Item;

namespace FrikanUtils.CustomItems;

/// <summary>
/// Custom goggles item. Already contains methods to detect when a player equips/dequips the goggles.
/// </summary>
public abstract class CustomScp1344Item : CustomUsableItem
{
    /// <inheritdoc />
    public override ItemType VisualType => ItemType.SCP1344;

    /// <inheritdoc />
    protected internal override void SubscribeEvents()
    {
        base.SubscribeEvents();
        Scp1344NetworkHandler.OnStatusChanged += StatusChanged;
    }

    /// <inheritdoc />
    protected internal override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        Scp1344NetworkHandler.OnStatusChanged -= StatusChanged;
    }

    /// <summary>
    /// Triggered when a player starts wearing the goggles.
    /// </summary>
    /// <param name="player">The player that is wearing the goggles</param>
    /// <param name="goggles">The item instance of the goggles that are equipped</param>
    protected virtual void OnWearGoggles(Player player, Scp1344Item goggles)
    {
    }

    /// <summary>
    /// Triggered when a player removes the goggles.
    /// </summary>
    /// <param name="player">The player that removed the goggles</param>
    /// <param name="goggles">The item instance of the goggles that are removed</param>
    protected virtual void OnRemovedGoggles(Player player, Scp1344Item goggles)
    {
    }

    private void StatusChanged(ushort serial, Scp1344Status status)
    {
        if (!Check(serial))
        {
            return;
        }

        var item = Item.Get(serial);
        if (item is not Scp1344Item goggles)
        {
            return;
        }

        var owner = item.CurrentOwner;
        if (status == Scp1344Status.Active)
        {
            OnWearGoggles(owner, goggles);
        }
        else if (status == Scp1344Status.Deactivating)
        {
            OnRemovedGoggles(owner, goggles);
        }
    }
}