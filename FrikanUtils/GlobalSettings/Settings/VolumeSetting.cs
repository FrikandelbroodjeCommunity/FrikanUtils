﻿using FrikanUtils.ServerSpecificSettings.Helpers;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings.Settings;

internal class VolumeSetting : IGlobalSetting
{
    public string Label => "Music volume";
    public bool ServerOnly => true;

    public SettingsBase Get(ushort settingId)
    {
        return new Slider(
            settingId,
            Label,
            0,
            200,
            UtilitiesPlugin.PluginConfig.AudioConfig.Volume,
            isServerOnly: ServerOnlyType.GlobalServerOnly
        ).RegisterChangedAction(UpdateValue);
    }

    public bool HasPermissions(Player player) => player.HasPermissions("frikanutils.audio");

    private void UpdateValue(Player player, float _, float value)
    {
        if (!HasPermissions(player))
        {
            return;
        }

        UtilitiesPlugin.PluginConfig.AudioConfig.Volume = value;
        UtilitiesPlugin.Save();
    }
}