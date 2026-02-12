using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

namespace FrikanUtils.CustomItems;

public abstract class CustomUsableItem : CustomItem
{
    /// <inheritdoc />
    protected internal override void SubscribeEvents()
    {
        base.SubscribeEvents();

        PlayerEvents.UsingItem += OnUsingItem;
        PlayerEvents.UsedItem += OnUsedItem;
        PlayerEvents.CancellingUsingItem += OnCancellingUse;
        PlayerEvents.CancelledUsingItem += OnCancelUse;
    }

    /// <inheritdoc />
    protected internal override void UnsubscribeEvents()
    {
        base.UnsubscribeEvents();
        
        PlayerEvents.UsingItem -= OnUsingItem;
        PlayerEvents.UsedItem -= OnUsedItem;
        PlayerEvents.CancellingUsingItem -= OnCancellingUse;
        PlayerEvents.CancelledUsingItem -= OnCancelUse;
    }

    protected virtual void OnUsingItem(PlayerUsingItemEventArgs ev)
    {
    }

    protected virtual void OnUsedItem(PlayerUsedItemEventArgs ev)
    {
    }

    protected virtual void OnCancellingUse(PlayerCancellingUsingItemEventArgs ev)
    {
    }

    protected virtual void OnCancelUse(PlayerCancelledUsingItemEventArgs ev)
    {
    }
}