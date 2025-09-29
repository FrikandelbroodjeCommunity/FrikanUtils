using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using LabApi.Features.Wrappers;
using Mirror;
using UnityEngine;
using Utils.NonAllocLINQ;
using KeycardItem = LabApi.Features.Wrappers.KeycardItem;
using KeycardPickup = LabApi.Features.Wrappers.KeycardPickup;
using Logger = LabApi.Features.Console.Logger;

namespace FrikanUtils.Keycard;

/// <summary>
/// Represents a custom keycard and its associated data.
/// There should at any time be at most 1 instance per custom keycard.
/// </summary>
public class CustomKeycard : IDoorPermissionProvider
{
    /// <summary>
    /// The base keycard this instance is a wrapper for.
    /// </summary>
    public KeycardItem Keycard { get; private set; }

    /// <summary>
    /// The serial number of the keycard.
    /// </summary>
    public ushort Serial => Keycard.Serial;

    /// <summary>
    /// Whether the keycard is currently equipped by a player.
    /// </summary>
    public bool IsHeld => Keycard.IsEquipped;

    /// <summary>
    /// Whether the keycard can open doors when it is thrown.
    /// </summary>
    public bool ThrownOpensDoors
    {
        get => Keycard.Base.OpenDoorsOnThrow;
        set
        {
            Keycard.Base.OpenDoorsOnThrow = value;
            if (Pickup.Get(Serial) is KeycardPickup keycardPickup)
            {
                keycardPickup.Base._openDoorsOnCollision = value;
            }
        }
    }

    /// <summary>
    /// When the <see cref="RepresentativeType"/> is set to something other than <see cref="ItemType.None"/>,
    /// the keycard will behave as that item when going through 914.
    /// </summary>
    public ItemType RepresentativeType;

    /// <summary>
    /// The inventory name of the item. The item serial will automatically be appended to the end of it.
    /// </summary>
    public string ItemName;

    /// <summary>
    /// The label displayed on the keycard.
    /// </summary>
    public string Label;

    /// <summary>
    /// The nametag displayed on the keycard
    /// </summary>
    public string Nametag;

    /// <summary>
    /// The rank detail used on MTF keycards (0-2)
    /// </summary>
    public int RankDetail;

    /// <summary>
    /// The custom wear level of the keycard.
    /// </summary>
    public byte Wear;

    /// <summary>
    /// THe permissions on the keycard.
    /// </summary>
    public KeycardLevels Permissions;

    /// <summary>
    /// The color of the label.
    /// </summary>
    public Color32 LabelColor;

    /// <summary>
    /// The color of the permission circles.
    /// </summary>
    public Color32? PermissionsColor;

    /// <summary>
    /// The tint of the keycard.
    /// </summary>
    public Color32 Tint;

    internal static readonly List<CustomKeycard> CustomKeycards = [];

    /// <summary>
    /// Apply all settings to the keycard.
    /// </summary>
    public void Apply()
    {
        CustomItemNameDetail._customText = $"{ItemName} [{Serial}]";
        CustomLabelDetail._customText = Label;
        NametagDetail._customNametag = Nametag;
        CustomRankDetail._index = Math.Min(Math.Max(RankDetail, 0), CustomKeycardUtilities.RankDetailMeshes.Length - 1);
        CustomWearDetail._customWearLevel = Wear;
        CustomPermsDetail._customLevels = Permissions;
        CustomLabelDetail._customColor = LabelColor;
        CustomPermsDetail._customColor = PermissionsColor;
        CustomTintDetail._customColor = Tint;

        try
        {
            var writer = KeycardDetailSynchronizer.GetPayloadWriter();
            foreach (var detail in Keycard.Base.Details)
            {
                if (detail is SyncedDetail syncedDetail)
                {
                    syncedDetail.WriteNewItem(Keycard.Base, writer);
                }
            }

            var payload = writer.ToArraySegment();
            KeycardDetailSynchronizer.Database[Serial] = payload;
            NetworkServer.SendToAll(new KeycardDetailSynchronizer.DetailsSyncMsg
            {
                Serial = Serial,
                Payload = payload
            });
        }
        catch (Exception e)
        {
            Logger.Warn($"Something went wrong while attempting to update keycard: {e}");
        }
    }

    /// <summary>
    /// If the given keycard is already a custom keycard, it will get the associated custom keycard instance.
    /// If the given keycard is not a custom keycard, it will convert it into a custom keycard with the same settings.
    /// </summary>
    /// <param name="keycard">Keycard to get/convert</param>
    /// <returns>The custom keycard</returns>
    public static CustomKeycard Create(KeycardItem keycard)
    {
        if (keycard.Base.Customizable && CustomKeycards.TryGetFirst(x => x.Serial == keycard.Serial, out var result))
        {
            return result;
        }

        result = new CustomKeycard
        {
            Keycard = keycard
        };

        result.ReadDetails();

        // If the card is already customizable, no need to do anything
        if (keycard.Base.Customizable)
        {
            CustomKeycards.Add(result);
            return result;
        }

        // If the keycard has no owner, cancel as we cannot replace the old card
        var player = keycard.CurrentOwner;
        var targetType = CustomKeycardUtilities.CustomTypeForKeycard(keycard.Type);
        if (player == null || targetType == ItemType.None) return null;

        // Store the previous card as the representative type
        result.RepresentativeType = keycard.Type;

        // Replace the keycard
        var holding = player.CurrentItem == keycard;
        player.RemoveItem(keycard);

        var newItem = player.AddItem(targetType);
        if (holding) player.CurrentItem = newItem;

        // Store the new card in the resulting instance
        result.Keycard = (KeycardItem)newItem;

        CustomKeycards.Add(result);
        return result;
    }

    /// <summary>
    /// Give a new custom keycard to a player.
    /// </summary>
    /// <param name="player">The player ot give the card to</param>
    /// <param name="newItem">The type of the keycard</param>
    /// <returns>The custom keycard</returns>
    public static CustomKeycard Create(Player player, ItemType newItem)
    {
        var createType = CustomKeycardUtilities.CustomTypeForKeycard(newItem);

        return createType == ItemType.None
            ? null
            : new CustomKeycard
            {
                Keycard = (KeycardItem)player.AddItem(createType),
                RepresentativeType = newItem == createType ? ItemType.None : newItem
            };
    }

    /// <summary>
    /// Copy the data onto a new custom keycard and give it to a player.
    /// </summary>
    /// <param name="keycard">The custom keycard to copy</param>
    /// <param name="player">The player to give the item to</param>
    /// <param name="targetKeycard">The type of the new custom keycard</param>
    /// <returns>The cloned custom keycard</returns>
    public static CustomKeycard CopyData(CustomKeycard keycard, Player player, ItemType targetKeycard)
    {
        var newInstance = Create(player, targetKeycard);
        newInstance.RepresentativeType = keycard.RepresentativeType;
        newInstance.ItemName = keycard.ItemName;
        newInstance.Label = keycard.Label;
        newInstance.Nametag = keycard.Nametag;
        newInstance.RankDetail = keycard.RankDetail;
        newInstance.Wear = keycard.Wear;
        newInstance.Permissions = keycard.Permissions;
        newInstance.LabelColor = keycard.LabelColor;
        newInstance.PermissionsColor = keycard.PermissionsColor;
        newInstance.Tint = keycard.Tint;

        newInstance.ThrownOpensDoors = keycard.ThrownOpensDoors;

        newInstance.Apply();
        CustomKeycards.Add(newInstance);
        return newInstance;
    }

    /// <summary>
    /// Try to get the custom keycard that belongs to a serial number.
    /// </summary>
    /// <param name="serial">Serial of the keycard</param>
    /// <param name="keycard">Found keycard or null</param>
    /// <returns>Whether a keycard was found</returns>
    public static bool TryGet(ushort serial, out CustomKeycard keycard)
    {
        return CustomKeycards.TryGetFirst(x => x.Serial == serial, out keycard);
    }

    public DoorPermissionFlags GetPermissions(IDoorPermissionRequester requester)
    {
        return Permissions.Permissions;
    }

    public PermissionUsed PermissionsUsedCallback => Keycard.Base.PermissionsUsedCallback;

    private void ReadDetails()
    {
        Permissions = Keycard.Levels;
        ItemName = Keycard.Base.Name;

        foreach (var detail in Keycard.Base.Details)
        {
            switch (detail)
            {
                case CustomLabelDetail:
                    if (Keycard.Base.KeycardGfx.KeycardLabels.Length == 0)
                    {
                        Logger.Warn("CustomLabelDetail: No label found");
                        break;
                    }

                    var keycardLabel = Keycard.Base.KeycardGfx.KeycardLabels[0];
                    Label = keycardLabel.text;
                    LabelColor = keycardLabel.color;
                    break;
                case CustomPermsDetail:
                    var materialInstance = Keycard.Base.KeycardGfx.Material.Instance;
                    var permsColor = materialInstance.GetColor(KeycardGfx.PermsColorHash);
                    PermissionsColor = permsColor.a <= float.Epsilon ? null : permsColor;
                    break;
                case CustomRankDetail:
                case PredefinedRankDetail:
                    var customRankMesh = Keycard.Base.KeycardGfx.RankFilter.sharedMesh;
                    RankDetail = customRankMesh == null
                        ? 0
                        : CustomKeycardUtilities.RankDetailMeshes.IndexOf(customRankMesh);
                    break;
                case CustomTintDetail:
                    Tint = Keycard.Base.KeycardGfx.Material.Instance.GetColor(KeycardGfx.TintColorHash);
                    break;
                case CustomWearDetail:
                    var elements = Keycard.Base.KeycardGfx.ElementVariants;
                    Wear = (byte)elements.IndexOf(elements.FirstOrDefault(x => x.activeSelf));
                    break;
                case NametagDetail:
                    if (Keycard.Base.KeycardGfx.NameFields.Length == 0)
                    {
                        Logger.Warn("NametagDetail: No label found");
                        break;
                    }

                    var nameLabel = Keycard.Base.KeycardGfx.NameFields[0];
                    Nametag = nameLabel.text;
                    break;
                case PredefinedTintDetail predefinedTintDetail:
                    Tint = predefinedTintDetail._color;
                    break;
                case PredefinedWearDetail predefinedWearDetail:
                    Wear = (byte)predefinedWearDetail._wearLevel;
                    break;
                case TranslatedLabelDetail translatedLabelDetail:
                    Label = Translations.Get(translatedLabelDetail._translation);
                    LabelColor = translatedLabelDetail._textColor;
                    break;
            }
        }
    }
}