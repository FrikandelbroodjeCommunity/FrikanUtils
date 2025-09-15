using System;
using FrikanUtils.ServerSpecificSettings.Helpers;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using MapGeneration.Holidays;

namespace FrikanUtils.GlobalSettings.InternalSettings;

public class HolidayOverrideSetting : IGlobalSetting
{
    public bool ServerOnly => true;

    public SettingsBase Get(byte settingId)
    {
        var values = Enum.GetValues(typeof(HolidayType)).ToArray<HolidayType>();
        return new TypedDropdown<HolidayType>(
            settingId,
            "[Debug] Override holiday",
            values,
            values.IndexOf(UtilitiesPlugin.PluginConfig.OverrideHoliday),
            hint: "Server will behave as if the event is running. " +
                  "Only intended for debugging purposes as this may cause some issues during the current round.",
            isServerOnly: ServerOnlyType.GlobalServerOnly
        ).RegisterChangedAction(UpdateValue);
    }

    public bool HasPermissions(Player player)
    {
        return player.HasPermissions("frikanutils.debug");
    }

    private static void UpdateValue(Player player, string value)
    {
        foreach (HolidayType holiday in Enum.GetValues(typeof(HolidayType)))
        {
            if (!value.Equals(holiday.ToString(), StringComparison.CurrentCultureIgnoreCase)) continue;

            UtilitiesPlugin.PluginConfig.OverrideHoliday = holiday;
            UtilitiesPlugin.Save();
            break;
        }
    }
}