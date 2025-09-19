using HarmonyLib;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace FrikanUtils.Npc.Patches;

[HarmonyPatch]
internal static class TargetPatches
{
    [HarmonyPatch(typeof(PlayerRolesUtils), nameof(PlayerRolesUtils.GetFaction), typeof(ReferenceHub))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool GetFaction(ref Faction __result, ReferenceHub hub)
    {
        var player = Player.Get(hub);
        if (player != null && BaseNpc.NpcsMapped.TryGetValue(player, out var data) && data.IgnoreTarget)
        {
            __result = Faction.Unclassified;
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(PlayerRolesUtils), nameof(PlayerRolesUtils.GetTeam), typeof(ReferenceHub))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool GetTeam(ref Team __result, ReferenceHub hub)
    {
        var player = Player.Get(hub);
        if (player != null && BaseNpc.NpcsMapped.TryGetValue(player, out var data) && data.IgnoreTarget)
        {
            __result = Team.OtherAlive;
            return false;
        }

        return true;
    }
}