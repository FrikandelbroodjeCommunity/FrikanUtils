using MapGeneration.Holidays;

namespace FrikanUtils.Utilities;

public static class HolidayUtilities
{
    public static HolidayType HolidayOverride => UtilitiesPlugin.PluginConfig.OverrideHoliday;

    /// <summary>
    /// Check whether an event is active, taking into account an override that can be used for debugging.
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <returns>Whether the given holiday type is active</returns>
    public static bool IsActive(this HolidayType type)
    {
        return (HolidayOverride != HolidayType.None && HolidayOverride == type) || HolidayUtils.IsHolidayActive(type);
    }
}