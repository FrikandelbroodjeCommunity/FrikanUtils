using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

namespace FrikanUtils.CustomItems;

/// <summary>
/// Custom usable item, contains event methods that are automatically registered for using items.
/// </summary>
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

    /// <summary>
    /// Default function that is triggered when using any item.
    /// </summary>
    /// <param name="ev">The event info</param>
    protected virtual void OnUsingItem(PlayerUsingItemEventArgs ev)
    {
    }

    /// <summary>
    /// Default function that is triggered when a player used any item.
    /// </summary>
    /// <param name="ev">The event info</param>
    protected virtual void OnUsedItem(PlayerUsedItemEventArgs ev)
    {
    }

    /// <summary>
    /// Default function that is triggered when cancelling the use of any item.
    /// </summary>
    /// <param name="ev">The event info</param>
    protected virtual void OnCancellingUse(PlayerCancellingUsingItemEventArgs ev)
    {
    }

    /// <summary>
    /// Default function that is triggered when a player canceled the use of any item.
    /// </summary>
    /// <param name="ev">The event info</param>
    protected virtual void OnCancelUse(PlayerCancelledUsingItemEventArgs ev)
    {
    }
}