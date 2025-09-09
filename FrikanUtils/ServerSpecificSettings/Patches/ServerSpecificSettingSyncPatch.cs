using System;
using HarmonyLib;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Mirror;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Patches;

[HarmonyPatch(typeof(ServerSpecificSettingsSync))]
public class ServerSpecificSettingSyncPatch
{
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

        // Update the SSS Base
        var reader = NetworkReaderPool.Get(msg.Payload);
        field.Base.DeserializeValue(reader);
        reader.Dispose();

        try
        {
            SSSEventHandler.OnValueReceived(player, field);
        }
        catch (Exception e)
        {
            Logger.Error($"Exception while receiving SSS packet for player {player.LogName}: {e}");
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