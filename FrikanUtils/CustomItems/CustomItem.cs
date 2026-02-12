using System.Collections.Generic;
using System.Linq;
using FrikanUtils.Spawnpoints;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace FrikanUtils.CustomItems;

/// <summary>
/// Represents an item with custom functionality.
/// </summary>
public abstract class CustomItem
{
    /// <summary>
    /// The unique ID of the item.
    /// </summary>
    public abstract string Id { get; }

    /// <summary>
    /// The name of the item displayed to the user.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// The description of the item displayed to the user.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// The type that is actually used for the item.
    /// </summary>
    public abstract ItemType VisualType { get; }

    /// <summary>
    /// The information to determine where the item should be spawned.
    /// </summary>
    public abstract SpawnLocation SpawnLocation { get; }

    /// <summary>
    /// Spawn the item as a pickup on the given location.
    /// </summary>
    /// <param name="position">Location to spawn the item at</param>
    /// <returns>The spawned pickup</returns>
    public Pickup SpawnItem(Vector3 position)
    {
        var pickup = Pickup.Create(VisualType, position);
        pickup.Spawn();

        CustomItemHandler.SpawnedItems[pickup.Serial] = this;

        return pickup;
    }

    /// <summary>
    /// Give the item to the desired player.
    /// </summary>
    /// <param name="player">Player to give the item to</param>
    /// <returns>The given item, or null</returns>
    public Item GrantItem(Player player)
    {
        var item = player.AddItem(VisualType);
        if (item != null)
        {
            CustomItemHandler.SpawnedItems[item.Serial] = this;
        }

        return item;
    }

    /// <summary>
    /// Subscribe to custom events for this item.
    /// </summary>
    protected internal virtual void SubscribeEvents()
    {
        PlayerEvents.PickingUpItem += PickingUp;
        PlayerEvents.PickedUpItem += PickedUp;
        PlayerEvents.DroppingItem += Dropping;
        PlayerEvents.DroppedItem += Dropped;
    }

    /// <summary>
    /// Unsubscribe to custom events for this item.
    /// </summary>
    protected internal virtual void UnsubscribeEvents()
    {
    }

    protected virtual void PickingUp(PlayerPickingUpItemEventArgs ev)
    {
    }

    protected virtual void PickedUp(PlayerPickedUpItemEventArgs ev)
    {
    }

    protected virtual void Dropping(PlayerDroppingItemEventArgs ev)
    {
    }

    protected virtual void Dropped(PlayerDroppedItemEventArgs ev)
    {
    }

    /// <summary>
    /// Check whether the given item is of this type.
    /// </summary>
    /// <param name="item">The item to check</param>
    /// <returns>Whether it is of this custom type</returns>
    protected bool Check(Item item) => Check(item.Serial);

    /// <summary>
    /// Check whether the given serial belongs to an item of this type.
    /// </summary>
    /// <param name="serial">The serial of the item to check</param>
    /// <returns>Whether it is of this custom type</returns>
    protected bool Check(ushort serial)
    {
        var customItem = CustomItemHandler.GetCustomForItem(serial);
        return customItem != null && customItem.Id == Id;
    }

    internal IEnumerable<ushort> SpawnItems()
    {
        return SpawnLocation.GetLocations().Select(x => SpawnItem(x).Serial);
    }

    internal void EquipHint(Player player)
    {
        player.SendHint($"<b>{Name}</b>\n<i>{Description}</i>");
    }

    internal void PickupHint(Player player)
    {
        player.SendHint($"You picked up a <b>{Name}</b>");
    }
}