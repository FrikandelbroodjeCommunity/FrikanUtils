using System;
using System.Collections.Generic;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Menus;

/// <summary>
/// Base for menus, can be used to create custom SSS menus.
/// Has some helper methods to get fields for this menu.
/// </summary>
public abstract class MenuBase : IEquatable<MenuBase>, IComparable<MenuBase>
{
    /// <summary>
    /// The name of the menu, displayed at the top of the menu.
    /// <b>This must be unique as this will help determine the field IDs</b>
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// The type of menu, will determine how the menu is displayed.
    /// </summary>
    public abstract MenuType Type { get; }

    /// <summary>
    /// How high should the menu appear in the menu.
    /// A higher priority will mean the menu is shown more to the top.
    /// </summary>
    public virtual int Priority => MenuPriority.Normal;

    /// <summary>
    /// Check whether a player has permission.
    /// Will only show the menu if a player has permission, otherwise the entire menu will be hidden.
    /// If you want to check permissions for certain parts, do a permission check in <see cref="GetSettings"/>!
    /// </summary>
    /// <param name="player">Player to check for</param>
    /// <returns>Whether the player has permission</returns>
    public virtual bool HasPermission(Player player) => true;

    /// <summary>
    /// Get the settings that should be displayed for this player.
    /// </summary>
    /// <param name="player">Player to load the settings for</param>
    /// <returns>Returns a list of settings to show</returns>
    public abstract IEnumerable<IServerSpecificSetting> GetSettings(Player player);

    /// <summary>
    /// Update this for all players. Any player who can see this menu will have their menu updated.
    ///
    /// When using force the menu will be updated immediately, otherwise the menu will be updated once the player opens the menu again.
    /// </summary>
    /// <param name="force">Whether to force the update</param>
    public void UpdateAll(bool force) => SSSHandler.UpdateAll(this, force);

    /// <summary>
    /// Update this menu for a specific player. The menu will only be updated if the player can see the menu.
    ///
    /// When using force the menu will be updated immediately, otherwise the menu will be updated once the player opens the menu again.
    /// </summary>
    /// <param name="player">Player to update the menu for</param>
    /// <param name="force">Whether to force the update</param>
    public void UpdateForPlayer(Player player, bool force) => SSSHandler.UpdatePlayer(player, this, force);

    /// <summary>
    /// Get all fields with the given ID.
    /// </summary>
    /// <param name="settingId">The unique ID of the fields</param>
    /// <typeparam name="T">The expected field type</typeparam>
    /// <returns>The found fields</returns>
    public IEnumerable<T> GetAllFields<T>(ushort settingId) where T : SettingsBase =>
        SSSHandler.GetAllFields<T>(this, settingId);

    /// <summary>
    /// Try to get a specific field for a player.
    /// </summary>
    /// <param name="player">The player to   get the field for</param>
    /// <param name="settingId">The unique ID of the field</param>
    /// <param name="result">The resulting field or null</param>
    /// <typeparam name="T">The expected field type</typeparam>
    /// <returns>Whether the field was found</returns>
    public bool TryGetField<T>(Player player, ushort settingId, out T result) where T : SettingsBase =>
        SSSHandler.TryGetField(player, this, settingId, out result);

    /// <summary>
    /// Get a specific field for a player. Will return null if the field could not be found or had the wrong type.
    /// </summary>
    /// <param name="player">The player to get the field for</param>
    /// <param name="settingId">The unique ID of the field</param>
    /// <typeparam name="T">The expected field type</typeparam>
    /// <returns>The found field or null</returns>
    public T GetField<T>(Player player, ushort settingId) where T : SettingsBase =>
        SSSHandler.GetField<T>(player, this, settingId);

    internal bool InternalHasPermission(Player player)
    {
        try
        {
            return HasPermission(player);
        }
        catch (Exception)
        {
            return false;
        }
    }

    // Comparison requirements (Sort based on priority)
    /// <inheritdoc />
    public int CompareTo(MenuBase other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Priority.CompareTo(other.Priority) * -1;
    }

    /// <inheritdoc />
    public bool Equals(MenuBase other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MenuBase)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Name != null ? Name.GetHashCode() : 0;
    }
}