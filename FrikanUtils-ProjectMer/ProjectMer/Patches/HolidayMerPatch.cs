using System;
using System.Collections.Generic;
using System.Linq;
using FrikanUtils.Utilities;
using HarmonyLib;
using LabApi.Features.Console;
using MapGeneration.Holidays;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable.Schematics;
using Utils.NonAllocLINQ;

namespace FrikanUtils.ProjectMer.Patches;

[HarmonyPatch]
internal static class HolidayMerPatch
{
    internal static readonly List<SchematicObject> ApplicableSchematics = [];

    [HarmonyPatch(typeof(SchematicObject), "CreateRecursiveFromID")]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool OnCreateObject(SchematicObject __instance, int id, List<SchematicBlockData> blocks)
    {
        // If it isn't applicable, no additional checks are needed
        if (!ApplicableSchematics.Contains(__instance) || blocks == null)
        {
            return true;
        }

        if (!blocks.TryGetFirst(x => x.ObjectId == id, out var block))
        {
            return true;
        }

        var separatorIndex = block.Name.IndexOf(';');
        if (separatorIndex < 0)
        {
            // The separator is not even included, so no need to check holidays
            return true;
        }

        var split = block.Name.Substring(0, separatorIndex).Split(',');
        foreach (var holiday in split)
        {
            if (Enum.TryParse(holiday, true, out HolidayType type) && type.IsActive())
            {
                return true;
            }
        }

        return false;
    }
}
