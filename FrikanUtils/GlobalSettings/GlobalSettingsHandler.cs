using System.Collections.Generic;
using LabApi.Features.Console;

namespace FrikanUtils.GlobalSettings;

public static class GlobalSettingsHandler
{
    internal static readonly List<GlobalSetting> ClientSettings = [];
    internal static readonly List<GlobalSetting> ServerSettings = [];

    public static void RegisterSetting(GlobalSetting setting)
    {
        if (setting.IsServerOnly)
        {
            ServerSettings.Add(setting);
        }
        else
        {
            ClientSettings.Add(setting);
        }
    }

    public static void UnregisterSetting(GlobalSetting setting)
    {
        var removed = setting.IsServerOnly ? ServerSettings.Remove(setting) : ClientSettings.Remove(setting);

        if (!removed)
        {
            Logger.Warn($"Could not remove setting {setting.Setting.Label}");
        }
    }
}