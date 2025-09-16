using System;
using System.Collections.Generic;
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
        if (!ApplicableSchematics.Contains(__instance) || block == null)
        {
            return true;
        }

        var separatorIndex = block.Name.IndexOf(';');
        if (separatorIndex < 0)
        {
            // The separator is not even included, so no need to check holidays
            return true;
        }

        var split = block.Name.Substring(0, separatorIndex + 1).Split(',');
        foreach (var holiday in split)
        {
            if (Enum.TryParse(holiday, out HolidayType type) && type.IsActive())
            {
                return true;
            }
        }

        return false;
    }
}