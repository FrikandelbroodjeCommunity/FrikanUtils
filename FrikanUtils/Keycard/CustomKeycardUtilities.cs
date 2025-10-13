using InventorySystem;
using InventorySystem.Items.Keycards;
using UnityEngine;

namespace FrikanUtils.Keycard;

/// <summary>
/// Set of helper functions to make working with custom keycards easier.
/// </summary>
public static class CustomKeycardUtilities
{
    /// <summary>
    /// List of custom keycard types.
    /// </summary>
    public static readonly ItemType[] CustomKeycards =
    [
        ItemType.KeycardCustomManagement,
        ItemType.KeycardCustomMetalCase,
        ItemType.KeycardCustomSite02,
        ItemType.KeycardCustomTaskForce
    ];

    /// <summary>
    /// List of rank detail meshes that can be used on MTF keycard.
    /// </summary>
    public static Mesh[] RankDetailMeshes
    {
        get
        {
            if (_rankDetailMeshes == null)
            {
                foreach (var availableItem in InventoryItemLoader.AvailableItems)
                {
                    if (availableItem.Value is not KeycardItem keycard) continue;

                    foreach (var detail in keycard.Details)
                    {
                        if (detail is CustomRankDetail rankDetail)
                        {
                            _rankDetailMeshes = rankDetail._options;
                        }
                    }
                }
            }

            return _rankDetailMeshes;
        }
    }

    private static Mesh[] _rankDetailMeshes;

    /// <summary>
    /// Get the associated custom keycard type for the given keycard type.
    /// </summary>
    /// <param name="keycardType">Original keycard</param>
    /// <returns>Associated custom keycard</returns>
    public static ItemType CustomTypeForKeycard(ItemType keycardType)
    {
        switch (keycardType)
        {
            case ItemType.KeycardJanitor:
            case ItemType.KeycardScientist:
            case ItemType.KeycardResearchCoordinator:
            case ItemType.KeycardContainmentEngineer:
            case ItemType.KeycardCustomSite02:
                return ItemType.KeycardCustomSite02;
            case ItemType.KeycardZoneManager:
            case ItemType.KeycardFacilityManager:
            case ItemType.KeycardCustomManagement:
                return ItemType.KeycardCustomManagement;
            case ItemType.KeycardGuard:
            case ItemType.KeycardCustomMetalCase:
                return ItemType.KeycardCustomMetalCase;
            case ItemType.KeycardMTFPrivate:
            case ItemType.KeycardMTFOperative:
            case ItemType.KeycardMTFCaptain:
            case ItemType.KeycardCustomTaskForce:
                return ItemType.KeycardCustomTaskForce;
            default:
                return ItemType.None;
        }
    }

    /// <summary>
    /// Get the default permissions color for a custom keycard.
    /// This needs to be done as passing a null color now results in white, instead of the default color.
    /// </summary>
    /// <returns></returns>
    public static Color32 GetDefaultPermsColor(ItemType keycardType)
    {
        var customType = CustomTypeForKeycard(keycardType);
        switch (customType)
        {
            case ItemType.KeycardCustomSite02:
            case ItemType.KeycardCustomManagement:
            case ItemType.KeycardCustomMetalCase:
                return new Color32(0, 0, 0, byte.MaxValue);
            case ItemType.KeycardCustomTaskForce:
                return new Color32(211, 181, 46, byte.MaxValue);
        }

        return new Color32(0, 0, 0, 0);
    }
}