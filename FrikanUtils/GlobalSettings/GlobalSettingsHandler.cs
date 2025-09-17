using System.Collections.Generic;
using FrikanUtils.ServerSpecificSettings;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

public static class GlobalSettingsHandler
{
    internal static readonly List<IGlobalSetting> ClientSettings = [];
    internal static readonly List<IGlobalSetting> ServerSettings = [];

    public static void RegisterSetting(IGlobalSetting setting)
    {
        if (setting.ServerOnly)
        {
            ServerSettings.Add(setting);
        }
        else
        {
            ClientSettings.Add(setting);
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

    public static IEnumerable<T> GetSettings<T>(IGlobalSetting setting) where T : SettingsBase
    {
        var targetMenu = setting.ServerOnly ? GlobalServerSettingsMenu.ServerId : GlobalClientSettingsMenu.ClientId;
        foreach (var player in Player.List)
        {
            var field = SSSHandler.GetFieldByLabel<T>(player, targetMenu, setting.Label);
            if (field != null)
            {
                yield return field;
            }
        }
    }
    
    public static T GetSetting<T>(Player player, IGlobalSetting setting) where T : SettingsBase
    {
        var targetMenu = setting.ServerOnly ? GlobalServerSettingsMenu.ServerId : GlobalClientSettingsMenu.ClientId;
        return SSSHandler.GetFieldByLabel<T>(player, targetMenu, setting.Label);
    }
}