using System;
using System.Collections.Generic;
using System.Linq;
using FrikanUtils.Utilities;
using HarmonyLib;
using MapGeneration.Holidays;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable.Schematics;

namespace FrikanUtils.ProjectMer.Patches;

[HarmonyPatch]
internal static class HolidayMerPatch
{
    internal static readonly List<SchematicObject> ApplicableSchematics = [];

    [HarmonyPatch(typeof(SchematicObject), nameof(SchematicObject.CreateObject))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool OnCreateObject(SchematicObject __instance, SchematicBlockData block)
    {
        // If it isn't applicable, no additional checks are needed
        if (!ApplicableSchematics.Contains(__instance))
        {
            return true;
        }

        var holidays = GetHolidayTypes(block.Name).ToArray();
        return holidays.IsEmpty() || // If it doesn't filter on holidays, skip it
               holidays.Any(x => x.IsActive()); // Otherwise check if one of the holidays is active
    }

    private static IEnumerable<HolidayType> GetHolidayTypes(string name)
    {
        var separatorIndex = name.IndexOf(';');
        if (separatorIndex < 0)
        {
            yield break;
        }

        var split = name.Substring(0, separatorIndex + 1).Split(',');
        foreach (var holiday in split)
        {
            if (Enum.TryParse(holiday, true, out HolidayType holidayType))
            {
                yield return holidayType;
            }
        }
    }
}