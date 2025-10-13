using MapGeneration.Holidays;

namespace FrikanUtils.Utilities;

/// <summary>
/// Utility to help with debugging holiday specific code.
/// In order to use the debug possibility, you should use the <see cref="IsActive"/> function for a holiday,
/// instead of the base game function.
/// </summary>
public static class HolidayUtilities
{
    /// <summary>
    /// The override for the holiday. When set will, use this instead of the base game value.
    /// To ignore this setting, set it to <see cref="HolidayType.None"/>.
    /// </summary>
    public static HolidayType HolidayOverride => UtilitiesPlugin.PluginConfig.OverrideHoliday;

    /// <summary>
    /// Check whether an event is active, taking into account an override that can be used for debugging.
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <returns>Whether the given holiday type is active</returns>
    public static bool IsActive(this HolidayType type)
    {
        return HolidayOverride == HolidayType.None ? HolidayUtils.IsHolidayActive(type) : HolidayOverride == type;
    }
}