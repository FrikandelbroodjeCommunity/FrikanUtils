using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

/// <summary>
/// Used to keep track of which global settings there are and the values for each player.
/// Register and unregister your global settings here to show/hide them.
/// </summary>
public static class GlobalSettingsHandler
{
    internal static readonly List<IGlobalSetting> ClientSettings = [];
    internal static readonly List<IGlobalSetting> ServerSettings = [new HolidayOverrideSetting()];
    internal static readonly GlobalServerSettingsMenu ServerMenu = new();
    internal static readonly GlobalClientSettingsMenu ClientMenu = new();

    private static readonly GlobalSettingComparer Comparer = new();

    /// <summary>
    /// Register a new setting, adding it to the global settings.
    /// <i>Will reload the Server Specific Settings for <b>all</b> players.</i>
    /// </summary>
    /// <param name="setting">Global setting to register</param>
    public static void RegisterSetting(IGlobalSetting setting)
    {
        if (setting.ServerOnly)
        {
            ServerSettings.Add(setting);
            ServerSettings.Sort(Comparer);
            ReloadServerSettings();
        }
        else
        {
            ClientSettings.Add(setting);
            ClientSettings.Sort(Comparer);
            ReloadClientSettings();
        }
    }

    /// <summary>
    /// Unregister a setting, removes it from the global settings.
    /// <i>Will reload the Server Specific Settings for <b>all</b> players.</i>
    /// </summary>
    /// <param name="setting">Global setting to unregister</param>
    public static void UnregisterSetting(IGlobalSetting setting)
    {
        var removed = setting.ServerOnly ? ServerSettings.Remove(setting) : ClientSettings.Remove(setting);

        if (!removed)
        {
            Logger.Warn($"Could not remove setting {setting}");
        }
    }

    /// <summary>
    /// Will reload the <see cref="GlobalServerSettingsMenu"/>, without forcing it.
    /// </summary>
    public static void ReloadServerSettings()
    {
        SSSHandler.UpdateAll(ServerMenu, false);
    }

    /// <summary>
    /// Will reload the <see cref="GlobalClientSettingsMenu"/>, forcing it to be immediately updated.
    /// </summary>
    public static void ReloadClientSettings()
    {
        SSSHandler.UpdateAll(ClientMenu, true);
    }

    /// <summary>
    /// Get the setting instances for a global setting. Used to retrieve the value for all players.
    /// Some players may be missing if they do not have enough premissions, or are not viewing the <see cref="GlobalServerSettingsMenu"/>.
    /// </summary>
    /// <param name="setting">Setting to retrieve the value for</param>
    /// <typeparam name="T">The type of setting to search for</typeparam>
    /// <returns>All found settings</returns>
    public static IEnumerable<T> GetSettings<T>(IGlobalSetting setting) where T : SettingsBase
    {
        return Player.List.Select(player => GetSetting<T>(player, setting)).Where(field => field != null);
    }

    /// <summary>
    /// Get the setting instance belonging to this player for a global setting. Used to retrieve the value for this player.
    /// It may return <c>null</c> if they do not have enough premissions, or are not viewing the <see cref="GlobalServerSettingsMenu"/>.
    /// </summary>
    /// <param name="player">Player to get the setting instance for</param>
    /// <param name="setting">The global setting to search for</param>
    /// <typeparam name="T">The type of setting to search for</typeparam>
    /// <returns>The found setting, or <c>null</c></returns>
    public static T GetSetting<T>(Player player, IGlobalSetting setting) where T : SettingsBase
    {
        var targetMenu = setting.ServerOnly ? GlobalServerSettingsMenu.ServerId : GlobalClientSettingsMenu.ClientId;
        var id = setting.ServerOnly
            ? ServerSettings.IndexOf(setting)
            : UtilitiesPlugin.PluginConfig.GlobalClientSettings.IndexOf(setting.Label);

        return id < 0 ? null : SSSHandler.GetField<T>(player, targetMenu, (ushort)id);
    }

    internal static void RegisterMenus()
    {
        SSSHandler.RegisterMenu(ClientMenu);
        SSSHandler.RegisterMenu(ServerMenu);
    }

    internal static void UnregisterMenus()
    {
        SSSHandler.UnregisterMenu(ClientMenu);
        SSSHandler.UnregisterMenu(ServerMenu);
    }
}