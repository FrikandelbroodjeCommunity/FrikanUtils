using System;
using System.Reflection;
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
            return false;
        }

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

        return false;
    }

    [HarmonyPatch(nameof(ServerSpecificSettingsSync.ClientProcessUpdateMsg))]
    [HarmonyPrefix]
    public static bool OnProcessUpdateMsg(SSSUpdateMessage msg)
    {
        return false;
    }
}