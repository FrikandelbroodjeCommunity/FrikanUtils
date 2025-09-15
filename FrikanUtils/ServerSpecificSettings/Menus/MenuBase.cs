using System;
using System.Collections.Generic;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Menus;

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
        return Name == other.Name;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MenuBase)obj);
    }

    public override int GetHashCode()
    {
        return Name != null ? Name.GetHashCode() : 0;
    }
}