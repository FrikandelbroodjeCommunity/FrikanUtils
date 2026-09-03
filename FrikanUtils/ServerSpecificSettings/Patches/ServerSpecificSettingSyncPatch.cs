using System;
using System.Linq;
using System.Reflection;
using FrikanUtils.ServerSpecificSettings.Settings;
using HarmonyLib;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Mirror;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Patches;

[HarmonyPatch(typeof(ServerSpecificSettingsSync))]
internal static class ServerSpecificSettingSyncPatch
{
    [HarmonyPrepare]
    public static bool OnPrepare(MethodBase _)
    {
        return UtilitiesPlugin.PluginConfig.UseServerSpecificSettings;
    }

    [HarmonyPatch(nameof(ServerSpecificSettingsSync.SendToAll))]
    [HarmonyPrefix]
    public static bool OnSendToAll()
    {
        SSSHandler.UpdateAll(true);
        return false;
    }

    [HarmonyPatch(nameof(ServerSpecificSettingsSync.SendToPlayersConditionally))]
    [HarmonyPrefix]
    public static bool OnSendToPlayersConditionally(Func<ReferenceHub, bool> filter)
    {
        foreach (var pair in SSSHandler.PlayerMenus.Where(x => filter.Invoke(x.Key.ReferenceHub)))
        {
            pair.Value.Update(true);
        }

        return false;
    }

    [HarmonyPatch(nameof(ServerSpecificSettingsSync.SendToPlayer), typeof(ReferenceHub))]
    [HarmonyPrefix]
    public static bool OnSendToPlayer(ReferenceHub hub)
    {
        var player = Player.Get(hub);
        SSSHandler.UpdatePlayer(player, true);
        return false;
    }

    [HarmonyPatch(nameof(ServerSpecificSettingsSync.SendToPlayer), typeof(ReferenceHub),
        typeof(ServerSpecificSettingBase[]), typeof(int?))]
    [HarmonyPrefix]
    public static bool OnSendToPlayer(ReferenceHub hub, ServerSpecificSettingBase[] collection)
    {
        var player = Player.Get(hub);
        if (SSSHandler.PlayerMenus.TryGetValue(player, out var menu))
        {
            menu.Update(true, collection);
        }

        return false;  
    }
    
    [HarmonyPatch(nameof(ServerSpecificSettingsSync.ServerProcessClientResponseMsg))]
    [HarmonyPrefix]
    public static bool OnReceiveMessage(NetworkConnection conn, SSSClientResponse msg)
    {
        if (!ReferenceHub.TryGetHub(conn.identity.gameObject, out var hub))
        {
            return false;
        }

        var player = Player.Get(hub);
        if (player == null || !SSSHandler.PlayerMenus.TryGetValue(player, out var menu))
        {
            return false;
        }

        if (msg.Id == -3) // Dynamic menu selector dropdown
        {
            var dropdownReader = NetworkReaderPool.Get(msg.Payload);
            menu.MenuSelection.DeserializeValue(dropdownReader);
            dropdownReader.Dispose();

            menu.SwitchWindow(menu.MenuSelection.SyncSelectionIndexRaw);
            return false;
        }

        var field = menu.GetSetting(msg.Id, msg.SettingType);
        if (field == null)
        {
            // Check if we have a field with this ID in the base game
            var baseField = menu.Rendering.FirstOrDefault(x => x.SettingId == msg.Id);
            if (baseField == null)
            {
                return false;
            }
            
            ProcessBaseSetting(msg, player, baseField);
        }
        else
        {
            ProcessSetting(msg, player, field);
        }

        return false;
    }

    [HarmonyPatch(nameof(ServerSpecificSettingsSync.ClientProcessUpdateMsg))]
    [HarmonyPrefix]
    public static bool OnProcessUpdateMsg(SSSUpdateMessage msg)
    {
        return false;
    }

    private static void ProcessSetting(SSSClientResponse msg, Player player, SettingsBase field)
    {
        var reader = NetworkReaderPool.Get(msg.Payload);

        try
        {
            SSSEventHandler.OnValueReceived(player, field, reader);
        }
        catch (Exception e)
        {
            if (UtilitiesPlugin.PluginConfig.Debug)
            {
                Logger.Error($"Exception while receiving SSS packet for player {player.LogName}: {e}");
            }
        }
        finally
        {
            reader.Dispose();
        }

    }

    private static void ProcessBaseSetting(SSSClientResponse msg, Player player, ServerSpecificSettingBase field)
    {
        var reader = NetworkReaderPool.Get(msg.Payload);
        ServerSpecificSettingsSync.ServerDeserializeClientResponse(player.ReferenceHub, field, reader);
        reader.Dispose();
    }
}