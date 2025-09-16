using System.Reflection;
using HarmonyLib;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorButtons;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Pickups;
using Mirror;
using UnityEngine;

namespace FrikanUtils.Keycard.Patches;

[HarmonyPatch(typeof(CollisionDetectionPickup))]
internal static class KeycardCollisionPatch
{
    [HarmonyPrepare]
    public static bool OnPrepare(MethodBase _)
    {
        return UtilitiesPlugin.PluginConfig.UseKeycardImprovements;
    }

    [HarmonyPatch(nameof(CollisionDetectionPickup.ProcessCollision))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    public static bool OnProcessCollision(CollisionDetectionPickup __instance, Collision collision)
    {
        if (!NetworkServer.active || !UtilitiesPlugin.PluginConfig.ImprovedCardDetection ||
            __instance is not KeycardPickup keycardPickup)
        {
            return true;
        }

        if (!collision.collider.TryGetComponent(out KeycardButton component) ||
            component.Target is not DoorVariant target ||
            !target.AllowInteracting(null, component.ColliderId))
        {
            return true;
        }

        IDoorPermissionProvider provider;
        var usingCustom = false;
        if (CustomKeycard.TryGet(__instance.Info.Serial, out var custom))
        {
            if (!custom.ThrownOpensDoors)
            {
                return true;
            }

            provider = custom;
            usingCustom = true;
        }
        else if (keycardPickup.Info.ItemId.TryGetTemplate(out KeycardItem itemProvider))
        {
            if (!keycardPickup._openDoorsOnCollision)
            {
                return true;
            }

            provider = itemProvider;
        }
        else
        {
            return true;
        }

        // If the door is locked
        if (target.ActiveLocks != 0)
        {
            switch (target)
            {
                case BasicDoor basicDoor:
                    basicDoor._remainingDeniedCooldown = basicDoor.DeniedCooldown;
                    basicDoor.RpcPlayBeepSound();
                    break;
                case CheckpointDoor checkpointDoor:
                    checkpointDoor._remainingDeniedCooldown = checkpointDoor.DeniedCooldown;
                    checkpointDoor.RpcPlayDeniedBeep();
                    break;
            }

            return false;
        }

        // Do an actual permission check
        if (target.CheckPermissions(provider, out var callback))
        {
            if (usingCustom)
            {
                target.NetworkTargetState = !target.TargetState;
                callback?.Invoke(target, true);
            }
            else // For some reason, attempting to change the target state doesn't work if it is not for a custom keycard...
            {
                return true;
            }
        }
        else
        {
            callback?.Invoke(target, false);

            switch (target)
            {
                case BasicDoor basicDoor:
                    basicDoor._remainingDeniedCooldown = basicDoor.DeniedCooldown;
                    basicDoor.RpcPlayBeepSound();
                    basicDoor.PlayDeniedButtonAnims(provider.GetPermissions(target));
                    break;
                case CheckpointDoor checkpointDoor:
                    checkpointDoor._remainingDeniedCooldown = checkpointDoor.DeniedCooldown;
                    checkpointDoor.RpcPlayDeniedBeep();
                    checkpointDoor.PlayDeniedButtonAnims(provider.GetPermissions(target));
                    break;
            }
        }

        return false;
    }
}