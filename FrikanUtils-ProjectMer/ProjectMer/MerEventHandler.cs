using System.Linq;
using FrikanUtils.ProjectMer.Patches;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

namespace FrikanUtils.ProjectMer;

internal static class MerEventHandler
{
    internal static void RegisterEvents()
    {
        PlayerEvents.SearchingToy += OnSearchingToy;
        PlayerEvents.SearchedToy += OnSearchedToy;
        ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
    }

    internal static void UnregisterEvents()
    {
        PlayerEvents.SearchingToy -= OnSearchingToy;
        PlayerEvents.SearchedToy -= OnSearchedToy;
        ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
    }

    private static void OnSearchingToy(PlayerSearchingToyEventArgs ev)
    {
        foreach (var info in MerUtilities.RegisteredPickups.Where(x => x.Toy == ev.Interactable.Base))
        {
            if (info.PickingUp != null && !info.PickingUp.Invoke(ev.Player, info.Id))
            {
                ev.IsAllowed = false;
            }
        }
    }

    private static void OnSearchedToy(PlayerSearchedToyEventArgs ev)
    {
        foreach (var info in MerUtilities.RegisteredPickups.Where(x => x.Toy == ev.Interactable.Base))
        {
            info.PickedUp?.Invoke(ev.Player, info.Id);
        }
    }

    private static void OnWaitingForPlayers()
    {
        MerUtilities.RegisteredPickups.Clear();
        HolidayMerPatch.ApplicableSchematics.Clear();
    }
}