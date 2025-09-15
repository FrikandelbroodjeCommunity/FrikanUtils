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

    internal static readonly List<MenuBase> StaticMenus = [];
    internal static readonly List<MenuBase> DynamicMenus = [];
    internal static readonly Dictionary<Player, PlayerMenu> PlayerMenus = new();

    public static void RegisterMenu(MenuBase menu)
    {
        if (UnregisterMenu(menu))
        {
            Logger.Warn("Found multiple menus with the same ID, overwriting the previous!");
        }

        if (menu.Type == MenuType.Dynamic)
        {
            DynamicMenus.Add(menu);
        }
        else
        {
            StaticMenus.Add(menu);
        }

        UpdateAll(menu);
    }

    public static bool UnregisterMenu(MenuBase menu)
    {
        if (StaticMenus.Contains(menu))
        {
            StaticMenus.Remove(menu);
            UpdateAll(menu);
            return true;
        }

        if (DynamicMenus.Contains(menu))
        {
            DynamicMenus.Remove(menu);
            UpdateAll(menu);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Update the given menu for all players. Any player who can see this menu will have their menu updated.
    /// </summary>
    /// <param name="menu">Menu to update</param>
    public static void UpdateAll(MenuBase menu)
    {
        foreach (var playerMenu in PlayerMenus.Values)
        {
            playerMenu.TryUpdate(menu);
        }
    }

    /// <summary>
    /// Update the given menu for a specific player. The menu will only be updated if the player can see the menu.
    /// </summary>
    /// <param name="player">Player to update the menu for</param>
    /// <param name="menu">Menu to update</param>
    public static void UpdatePlayer(Player player, MenuBase menu)
    {
        if (PlayerMenus.TryGetValue(player, out var playerMenu))
        {
            playerMenu.TryUpdate(menu);
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
        if (StaticMenus.Contains(menu) || DynamicMenus.Contains(menu))
        {
            return from pair in PlayerMenus where pair.Value.HasMenu(menu) select pair.Key;
        }

        return [];
    }

    /// <summary>
    /// Get all fields with the given ID.
    /// </summary>
    /// <param name="settingId">The unique ID of the fields</param>
    /// <typeparam name="T">THe expected field type</typeparam>
    /// <returns>The found fields</returns>
    public static IEnumerable<T> GetAllFields<T>(string settingId) where T : SettingsBase
    {
        return PlayerMenus.Values
            .Select(playerMenu => playerMenu.GetSetting<T>(settingId))
            .Where(field => field != null);
    }

    /// <summary>
    /// Try to get a specific field for a player.
    /// </summary>
    /// <param name="player">The player to get the field for</param>
    /// <param name="settingId">The unique ID of the field</param>
    /// <param name="result">The resulting field or null</param>
    /// <typeparam name="T">The expected field type</typeparam>
    /// <returns>Whether the field was found</returns>
    public static bool TryGetField<T>(Player player, string settingId, out T result)
        where T : SettingsBase
    {
        result = GetField<T>(player, settingId);
        return result != null;
    }

    /// <summary>
    /// Get a specific field for a player. Will return null if the field could not be found or had the wrong type.
    /// </summary>
    /// <param name="player">The player to get the field for</param>
    /// <param name="settingId">The unique ID of the field</param>
    /// <typeparam name="T">The expected field type</typeparam>
    /// <returns>The found field or null</returns>
    public static T GetField<T>(Player player, string settingId) where T : SettingsBase
    {
        return PlayerMenus.TryGetValue(player, out var playerMenu) ? playerMenu.GetSetting<T>(settingId) : null;
    }

    internal static void CreatePlayer(Player player)
    {
        var menu = new PlayerMenu(player);
        PlayerMenus.Add(player, menu);
        menu.Update();
    }

    internal static void DestroyPlayer(Player player)
    {
        PlayerMenus.Remove(player);
    }
}