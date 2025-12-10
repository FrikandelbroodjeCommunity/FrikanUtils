using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Utils.NonAllocLINQ;

namespace FrikanUtils.CustomItems;

/// <summary>
/// Manages custom items.
/// </summary>
public static class CustomItemHandler
{
    internal static readonly Dictionary<ushort, CustomItem> SpawnedItems = [];

    private static readonly List<CustomItem> CustomItems = [];

    /// <summary>
    /// Registers a custom item, enabling the use in game. 
    /// </summary>
    /// <param name="item">The item to register</param>
    public static void RegisterCustomItem(CustomItem item)
    {
        if (CustomItems.AddIfNotContains(item))
        {
            item.SubscribeEvents();
        }
    }

    /// <summary>
    /// Unregisters a custom item, preventing the use in game.
    /// Any items that are already granted will stay in the inventory, but no longer detected as the custom item. 
    /// </summary>
    /// <param name="item">The item to unregister</param>
    public static void UnregisterCustomItem(CustomItem item)
    {
        CustomItems.Remove(item);
        item.UnsubscribeEvents();
    }

    /// <summary>
    /// Get the custom item class for a given item.
    /// </summary>
    /// <param name="item">The item to get the custom item for</param>
    /// <returns>The custom item or null</returns>
    public static CustomItem GetCustomForItem(Item item) => GetCustomForItem(item.Serial);

    /// <summary>
    /// Get the custom item class for a given item serial.
    /// </summary>
    /// <param name="serial">The serial of the item to get the custom item for</param>
    /// <returns>The custom item or null</returns>
    public static CustomItem GetCustomForItem(ushort serial)
    {
        return SpawnedItems.TryGetValue(serial, out var value) ? value : null;
    }

    internal static void RegisterEvents()
    {
        ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
        ServerEvents.RoundStarted += OnRoundStart;
        PlayerEvents.UsedItem += OnUsedItem;
        PlayerEvents.UsingItem += OnUsingItem;
        PlayerEvents.CancelledUsingItem += OnCancelUse;
        PlayerEvents.ChangedItem += OnEquip;
        PlayerEvents.PickedUpItem += OnPickup;
    }

    internal static void UnregisterEvents()
    {
        ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
        ServerEvents.RoundStarted -= OnRoundStart;
        PlayerEvents.UsedItem -= OnUsedItem;
        PlayerEvents.UsingItem -= OnUsingItem;
        PlayerEvents.CancelledUsingItem -= OnCancelUse;
        PlayerEvents.ChangedItem -= OnEquip;
        PlayerEvents.PickedUpItem -= OnPickup;
    }

    private static void OnWaitingForPlayers()
    {
        SpawnedItems.Clear();
    }

    private static void OnRoundStart()
    {
        foreach (var item in CustomItems)
        {
            foreach (var id in item.SpawnItems())
            {
                SpawnedItems[id] = item;
            }
        }
    }

    private static void OnUsedItem(PlayerUsedItemEventArgs ev)
    {
        if (SpawnedItems.TryGetValue(ev.UsableItem.Serial, out var customItem))
        {
            customItem.OnUsedItem(ev.Player, ev.UsableItem);
        }
    }

    private static void OnUsingItem(PlayerUsingItemEventArgs ev)
    {
        if (!ev.IsAllowed)
        {
            return;
        }

        if (SpawnedItems.TryGetValue(ev.UsableItem.Serial, out var customItem))
        {
            ev.IsAllowed = customItem.OnUsingItem(ev.Player, ev.UsableItem);
        }
    }

    private static void OnCancelUse(PlayerCancelledUsingItemEventArgs ev)
    {
        if (SpawnedItems.TryGetValue(ev.UsableItem.Serial, out var customItem))
        {
            customItem.OnCancelUse(ev.Player, ev.UsableItem);
        }
    }

    private static void OnEquip(PlayerChangedItemEventArgs ev)
    {
        if (ev.NewItem == null)
        {
            return;
        }

        if (SpawnedItems.TryGetValue(ev.NewItem.Serial, out var customItem))
        {
            customItem.EquipHint(ev.Player);
        }
    }

    private static void OnPickup(PlayerPickedUpItemEventArgs ev)
    {
        if (SpawnedItems.TryGetValue(ev.Item.Serial, out var customItem))
        {
            customItem.PickupHint(ev.Player);
        }
    }
}