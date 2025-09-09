using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using Respawning.Objectives;
using Scp914;

namespace FrikanUtils.Keycard;

/// <summary>
/// System to make custom keycards work in 914 if an equivalent keycard is known.
/// </summary>
internal static class CustomKeycardEventHandler
{
    internal static void RegisterEvents()
    {
        Scp914Events.ProcessingInventoryItem += OnInventoryProcessingItem;
        Scp914Events.ProcessingPickup += OnPickupProcessingItem;
    }

    internal static void UnregisterEvents()
    {
        Scp914Events.ProcessingInventoryItem -= OnInventoryProcessingItem;
        Scp914Events.ProcessingPickup -= OnPickupProcessingItem;
    }

    private static void OnInventoryProcessingItem(Scp914ProcessingInventoryItemEventArgs ev)
    {
        if (!ev.IsAllowed || !CustomKeycardUtilities.CustomKeycards.Contains(ev.Item.Type))
        {
            return;
        }

        ev.IsAllowed = false; // Always disallow custom keycards as they cannot be upgraded

        // Check if the card is represented and we can get a processor for it
        if (!Keycard.CustomKeycard.TryGet(ev.Item.Serial, out var keycardData) ||
            keycardData.RepresentativeType == ItemType.None ||
            !Scp914Upgrader.TryGetProcessor(keycardData.RepresentativeType, out var processor))
        {
            return;
        }

        ev.Player.RemoveItem(ev.Item);

        var tempItem = ev.Player.AddItem(keycardData.RepresentativeType);
        CustomKeycard.CustomKeycards.Remove(keycardData);

        var scp914Result = processor.UpgradeInventoryItem(ev.KnobSetting, tempItem.Base);
        ProcessResults(scp914Result);
    }

    private static void OnPickupProcessingItem(Scp914ProcessingPickupEventArgs ev)
    {
        if (!ev.IsAllowed || !CustomKeycardUtilities.CustomKeycards.Contains(ev.Pickup.Type))
        {
            return;
        }

        ev.IsAllowed = false;

        // Check if the card is represented and we can get a processor for it
        if (!CustomKeycard.TryGet(ev.Pickup.Serial, out var keycardData) ||
            keycardData.RepresentativeType == ItemType.None ||
            !Scp914Upgrader.TryGetProcessor(keycardData.RepresentativeType, out var processor))
        {
            return;
        }

        var tempPickup = Pickup.Create(ev.Pickup.Type, ev.Pickup.Position);
        ev.Pickup.Destroy();
        CustomKeycard.CustomKeycards.Remove(keycardData);

        var scp914Result = processor.UpgradePickup(ev.KnobSetting, tempPickup.Base);
        ProcessResults(scp914Result);
    }

    private static void ProcessResults(Scp914Result result)
    {
        if (result.ResultingItems != null)
        {
            foreach (var item in result.ResultingItems)
            {
                ScpItemPickupObjective.BlacklistedItems.Add(item.ItemSerial);
            }
        }

        if (result.ResultingPickups != null)
        {
            foreach (var pickup in result.ResultingPickups)
            {
                ScpItemPickupObjective.BlacklistedItems.Add(pickup.Info.Serial);
            }
        }
    }
}