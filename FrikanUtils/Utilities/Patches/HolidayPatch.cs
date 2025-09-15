using HarmonyLib;
using MapGeneration.Holidays;

namespace FrikanUtils.Utilities.Patches;

[HarmonyPatch]
internal static class HolidayPatch
{
    internal static HolidayType Holiday;

    [HarmonyPatch(typeof(HolidayUtils), nameof(HolidayUtils.GetActiveHoliday))]
    [HarmonyPrefix]
    public static bool OverrideHoliday(ref HolidayType __result)
    {
        var holiday = UtilitiesPlugin.PluginConfig.OverrideHoliday;
        if (holiday == HolidayType.None)
        {
            return true;
        }

        __result = holiday;
        return false;
    }
}