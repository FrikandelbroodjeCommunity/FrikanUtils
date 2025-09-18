using FrikanUtils.Npc.Enums;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

namespace FrikanUtils.Npc;

internal static class NpcEventHandler
{
    internal static void RegisterEvents()
    {
        PlayerEvents.Death += OnPlayerDeath;
    }

    internal static void UnregisterEvents()
    {
        PlayerEvents.Death -= OnPlayerDeath;
    }

    private static void OnPlayerDeath(PlayerDeathEventArgs ev)
    {
        if (!ev.Player.GameObject.TryGetComponent(out BaseNpcComponent npcComponent) || 
            !npcComponent.NpcData.DestroyOnDeath)
        {
            return;
        }

        npcComponent.NpcData.Destroy(DestroyReason.Died);
    }
}