using System;
using System.Collections.Generic;
using System.Linq;
using FrikanUtils.GlobalSettings.InternalSettings;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;
using MapGeneration.Holidays;

namespace FrikanUtils.GlobalSettings;

internal static class InternalGlobalSettings
{
    private static readonly IGlobalSetting[] InternalSettings = GenerateInternalSettings().ToArray();

    internal static void RegisterInternalSettings()
    {
        foreach (var setting in InternalSettings)
        {
            GlobalSettingsHandler.RegisterSetting(setting);
        }
    }

    internal static void UnregisterInternalSettings()
    {
        foreach (var setting in InternalSettings)
        {
            GlobalSettingsHandler.UnregisterSetting(setting);
        }
    }

    private static IEnumerable<IGlobalSetting> GenerateInternalSettings()
    {
        yield return new HolidayOverrideSetting();
    }

    private static void OnSetHoliday(Player player, string _)
    {
        
    }
}