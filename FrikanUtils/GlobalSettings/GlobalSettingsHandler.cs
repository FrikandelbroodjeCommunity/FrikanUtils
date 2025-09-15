using System.Collections.Generic;
using LabApi.Features.Console;

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
}