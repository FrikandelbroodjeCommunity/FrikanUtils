using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Menus;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings;

public static class SSSHandler
{
    internal const int LowestReservedId = -4;

    internal static readonly List<MenuBase> RegisteredMenus = [];
    internal static readonly Dictionary<Player, PlayerMenu> PlayerMenus = new();

    public static void RegisterMenu(MenuBase menu)
    {
        if (UnregisterMenu(menu))
        {
            Logger.Warn("Found multiple menus with the same ID, overwriting the previous!");
        }

        RegisteredMenus.Add(menu);
        UpdateAll(menu, true);
    }

    public static bool UnregisterMenu(MenuBase menu)
    {
        if (!RegisteredMenus.Remove(menu)) return false;

        UpdateAll(menu, true);
        return true;
    }

    /// <summary>
    /// Update the given menu for all players. Any player who can see this menu will have their menu updated.
    ///
    /// When using force the menu will be updated immediately, otherwise the menu will be updated once the player opens the menu again.
    /// </summary>
    /// <param name="menu">Menu to update</param>
    /// <param name="force">Whether to force the update</param>
    public static void UpdateAll(MenuBase menu, bool force)
    {
        foreach (var playerMenu in PlayerMenus.Values)
        {
            playerMenu.TryUpdate(menu, force);
        }
    }

    /// <summary>
    /// Update the given menu for a specific player. The menu will only be updated if the player can see the menu.
    ///
    /// When using force the menu will be updated immediately, otherwise the menu will be updated once the player opens the menu again.
    /// </summary>
    /// <param name="player">Player to update the menu for</param>
    /// <param name="menu">Menu to update</param>
    /// <param name="force">Whether to force the update</param>
    public static void UpdatePlayer(Player player, MenuBase menu, bool force)
    {
        if (PlayerMenus.TryGetValue(player, out var playerMenu))
        {
            playerMenu.TryUpdate(menu, force);
        }
    }

    /// <summary>
    /// Get a list of all players that have the given menu open. Can be used for soft-refreshing.
    /// If the menu is not registered, no players will be given.
    /// </summary>
    /// <param name="menu">Menu to get the players for</param>
    /// <returns>Players that have the menu open</returns>
    public static IEnumerable<Player> GetAllPlayers(MenuBase menu)
    {
        if (RegisteredMenus.Contains(menu))
        {
            return from pair in PlayerMenus where pair.Value.HasMenu(menu) select pair.Key;
        }

        return [];
    }

    /// <summary>
    /// Get all fields with the given ID.
    /// </summary>
    /// <param name="menu">The menu to search in</param>
    /// <param name="settingId">The unique ID of the fields</param>
    /// <typeparam name="T">The expected field type</typeparam>
    /// <returns>The found fields</returns>
    public static IEnumerable<T> GetAllFields<T>(MenuBase menu, ushort settingId) where T : SettingsBase
        => GetAllFields<T>(menu.Name, settingId);

    /// <summary>
    /// Get all fields with the given ID.
    /// </summary>
    /// <param name="menu">The exact name of the menu</param>
    /// <param name="settingId">The unique ID of the fields</param>
    /// <typeparam name="T">The expected field type</typeparam>
    /// <returns>The found fields</returns>
    public static IEnumerable<T> GetAllFields<T>(string menu, ushort settingId) where T : SettingsBase
    {
        return PlayerMenus.Values
            .Select(playerMenu => playerMenu.GetSetting<T>(menu, settingId))
            .Where(field => field != null);
    }

    /// <summary>
    /// Try to get a specific field for a player.
    /// </summary>
    /// <param name="player">The player to get the field for</param>
    /// <param name="menu">The menu to search in</param>
    /// <param name="settingId">The unique ID of the field</param>
    /// <param name="result">The resulting field or null</param>
    /// <typeparam name="T">The expected field type</typeparam>
    /// <returns>Whether the field was found</returns>
    public static bool TryGetField<T>(Player player, MenuBase menu, ushort settingId, out T result)
        where T : SettingsBase
        => TryGetField(player, menu.Name, settingId, out result);

    /// <summary>
    /// Try to get a specific field for a player.
    /// </summary>
    /// <param name="player">The player to get the field for</param>
    /// <param name="menu">The exact name of the menu</param>
    /// <param name="settingId">The unique ID of the field</param>
    /// <param name="result">The resulting field or null</param>
    /// <typeparam name="T">The expected field type</typeparam>
    /// <returns>Whether the field was found</returns>
    public static bool TryGetField<T>(Player player, string menu, ushort settingId, out T result)
        where T : SettingsBase
    {
        result = GetField<T>(player, menu, settingId);
        return result != null;
    }

    /// <summary>
    /// Get a specific field for a player. Will return null if the field could not be found or had the wrong type.
    /// </summary>
    /// <param name="player">The player to get the field for</param>
    /// <param name="menu">The menu to search in</param>
    /// <param name="settingId">The unique ID of the field</param>
    /// <typeparam name="T">The expected field type</typeparam>
    /// <returns>The found field or null</returns>
    public static T GetField<T>(Player player, MenuBase menu, ushort settingId) where T : SettingsBase
        => GetField<T>(player, menu.Name, settingId);

    /// <summary>
    /// Get a specific field for a player. Will return null if the field could not be found or had the wrong type.
    /// </summary>
    /// <param name="player">The player to get the field for</param>
    /// <param name="menu">The exact name of the menu</param>
    /// <param name="settingId">The unique ID of the field</param>
    /// <typeparam name="T">The expected field type</typeparam>
    /// <returns>The found field or null</returns>
    public static T GetField<T>(Player player, string menu, ushort settingId) where T : SettingsBase
    {
        return PlayerMenus.TryGetValue(player, out var playerMenu) ? playerMenu.GetSetting<T>(menu, settingId) : null;
    }

    internal static void CreatePlayer(Player player)
    {
        var menu = new PlayerMenu(player);
        PlayerMenus.Add(player, menu);
        menu.Update(true);
    }

    internal static void DestroyPlayer(Player player)
    {
        PlayerMenus.Remove(player);
    }
}