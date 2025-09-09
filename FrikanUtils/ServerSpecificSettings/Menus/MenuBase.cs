using System;
using System.Collections.Generic;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Menus;

/// <summary>
/// Do <b>NOT</b> inherit from this class.
/// Use StaticMenuBase or DynamicMenuBase instead!
/// </summary>
public abstract class MenuBase : IEquatable<MenuBase>, IComparable<MenuBase>
{
    /// <summary>
    /// The internal ID of this menu. Should be unique as duplicate IDs are not allowed to be registered.
    /// </summary>
    public abstract string MenuId { get; }

    /// <summary>
    /// Whether this menu is a dynamic menu.
    /// In case of a dynamic menu, the contents given by <code>GetSettings</code> can be changed.
    /// When using a static menu, the contents given by <code>GetSettings</code> cannot be changed,
    /// not even depending on the player given as at the moment the menu is registered, the fields must be known.
    /// </summary>
    public abstract bool IsDynamic { get; }

    /// <summary>
    /// Whether to force show this dynamic menu all the way at the top.
    /// Only used when IsDynamic is set to true!
    /// </summary>
    public abstract bool IsForced { get; }

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
    public abstract IEnumerable<SettingsBase> GetSettings(Player player);

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
    public int CompareTo(MenuBase other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Priority.CompareTo(other.Priority) * -1;
    }

    public bool Equals(MenuBase other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return MenuId == other.MenuId;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MenuBase)obj);
    }

    public override int GetHashCode()
    {
        return MenuId != null ? MenuId.GetHashCode() : 0;
    }
}