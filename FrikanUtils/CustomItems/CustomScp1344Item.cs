using InventorySystem.Items.Usables.Scp1344;
using LabApi.Features.Wrappers;
using Scp1344Item = LabApi.Features.Wrappers.Scp1344Item;

namespace FrikanUtils.CustomItems;

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

    protected virtual void OnWearGoggles(Player player, Scp1344Item goggles)
    {
    }

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