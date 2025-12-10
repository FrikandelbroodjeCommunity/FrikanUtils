using System.Collections.Generic;
using System.Linq;
using FrikanUtils.Spawnpoints;
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
    }

    /// <summary>
    /// Unsubscribe to custom events for this item.
    /// </summary>
    protected internal virtual void UnsubscribeEvents()
    {
    }

    /// <summary>
    /// Called when a player uses an item of this custom item type.
    /// </summary>
    /// <param name="player">The player that used the item</param>
    /// <param name="item">The item that was used</param>
    protected internal virtual void OnUsedItem(Player player, UsableItem item)
    {
    }

    /// <summary>
    /// Called when a player starts using an item of this custom item type.
    /// Return false to prevent the actual item from being used.
    /// </summary>
    /// <param name="player">The player that is using the item</param>
    /// <param name="item">The item that is being used</param>
    /// <returns>Whether the usage is permitted</returns>
    protected internal virtual bool OnUsingItem(Player player, UsableItem item) => true;

    /// <summary>
    /// Called when the player cancels their usage of the item.
    /// </summary>
    /// <param name="player">The player that cancelled the use</param>
    /// <param name="item">The item the usage of has been cancelled</param>
    protected internal virtual void OnCancelUse(Player player, UsableItem item)
    {
    }

    /// <summary>
    /// Called when the player picks up an instance of this item.
    /// </summary>
    /// <param name="player">The player that picked up the item</param>
    /// <param name="item">The item that was picked up</param>
    protected internal virtual void OnPickup(Player player, Item item)
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