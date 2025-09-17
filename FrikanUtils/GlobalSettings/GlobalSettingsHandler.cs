using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

public static class GlobalSettingsHandler
{
    internal static readonly List<IGlobalSetting> ClientSettings = [];
    internal static readonly List<IGlobalSetting> ServerSettings = [];
    private static readonly GlobalSettingComparer Comparer = new();

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

    public static void UnregisterSetting(IGlobalSetting setting)
    {
        var removed = setting.ServerOnly ? ServerSettings.Remove(setting) : ClientSettings.Remove(setting);

        if (!removed)
        {
            Logger.Warn($"Could not remove setting {setting}");
        }
    }

    public static void ReloadServerSettings()
    {
        SSSHandler.UpdateAll(InternalGlobalSettings.ServerMenu);
    }

    public static void ReloadClientSettings()
    {
        SSSHandler.UpdateAll(InternalGlobalSettings.ClientMenu);
    }

    public static IEnumerable<T> GetSettings<T>(IGlobalSetting setting) where T : SettingsBase
    {
        return Player.List.Select(player => GetSetting<T>(player, setting)).Where(field => field != null);
    }

    public static T GetSetting<T>(Player player, IGlobalSetting setting) where T : SettingsBase
    {
        var targetMenu = setting.ServerOnly ? GlobalServerSettingsMenu.ServerId : GlobalClientSettingsMenu.ClientId;
        var id = setting.ServerOnly
            ? ServerSettings.IndexOf(setting)
            : UtilitiesPlugin.PluginConfig.GlobalClientSettings.IndexOf(setting.Label);

        return id < 0 ? null : SSSHandler.GetField<T>(player, targetMenu, (ushort)id);
    }
}