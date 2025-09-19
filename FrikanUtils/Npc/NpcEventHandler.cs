using FrikanUtils.Npc.Enums;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp096Events;
using LabApi.Events.Arguments.Scp173Events;
using LabApi.Events.Handlers;

namespace FrikanUtils.Npc;

internal static class NpcEventHandler
{
    internal static void RegisterEvents()
    {
        PlayerEvents.Death += OnPlayerDeath;
        Scp096Events.AddingTarget += OnTriggering096;
        Scp173Events.AddingObserver += OnLooking173;
        PlayerEvents.Escaping += OnEscaping;
    }

    internal static void UnregisterEvents()
    {
        PlayerEvents.Death -= OnPlayerDeath;
        Scp096Events.AddingTarget -= OnTriggering096;
        Scp173Events.AddingObserver -= OnLooking173;
        PlayerEvents.Escaping -= OnEscaping;
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

    private static void OnTriggering096(Scp096AddingTargetEventArgs ev)
    {
        if (BaseNpc.NpcsMapped.TryGetValue(ev.Player, out var npcData) && npcData.IgnoreScp096)
        {
            ev.IsAllowed = false;
        }
    }

    private static void OnLooking173(Scp173AddingObserverEventArgs ev)
    {
        if (BaseNpc.NpcsMapped.TryGetValue(ev.Player, out var npcData) && npcData.IgnoreScp173)
        {
            ev.IsAllowed = false;
        }
    }

    private static void OnEscaping(PlayerEscapingEventArgs ev)
    {
        if (BaseNpc.NpcsMapped.TryGetValue(ev.Player, out var npcData) && npcData.CanEscape)
        {
            ev.IsAllowed = false;
        }
    }
}