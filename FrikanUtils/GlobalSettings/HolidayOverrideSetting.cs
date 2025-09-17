using System;
using FrikanUtils.ServerSpecificSettings.Helpers;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using MapGeneration.Holidays;

namespace FrikanUtils.GlobalSettings;

public class HolidayOverrideSetting : IGlobalSetting
{
    public string Label => "[Debug] Override holiday";
    public bool ServerOnly => true;

    public SettingsBase Get(ushort settingId)
    {
        var values = Enum.GetValues(typeof(HolidayType)).ToArray<HolidayType>();
        return new TypedDropdown<HolidayType>(
            settingId,
            Label,
            values,
            values.IndexOf(UtilitiesPlugin.PluginConfig.OverrideHoliday),
            hint: "Server will behave as if the event is running. " +
                  "Only intended for debugging purposes as this may cause some issues during the current round.",
            isServerOnly: ServerOnlyType.GlobalServerOnly
        ).RegisterChangedAction(UpdateValue);
    }

    public bool HasPermissions(Player player) => player.HasPermissions("frikanutils.debug");

    private void UpdateValue(Player player, string value)
    {
        if (!HasPermissions(player))
        {
            return;
        }

        foreach (HolidayType holiday in Enum.GetValues(typeof(HolidayType)))
        {
            if (!value.Equals(holiday.ToString(), StringComparison.CurrentCultureIgnoreCase)) continue;

            UtilitiesPlugin.PluginConfig.OverrideHoliday = holiday;
            UtilitiesPlugin.Save();
            break;
        }
    }
}