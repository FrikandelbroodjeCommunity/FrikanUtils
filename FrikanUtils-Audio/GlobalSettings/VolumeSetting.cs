using FrikanUtils.ServerSpecificSettings.Helpers;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

public class VolumeSetting : IGlobalSetting
{
    public string Label => "Music volume";
    public bool ServerOnly => true;

    public SettingsBase Get(byte settingId)
    {
        return new Slider(
            settingId,
            Label,
            0,
            200,
            AudioPlugin.Instance.Config.Volume,
            isServerOnly: ServerOnlyType.GlobalServerOnly
        ).RegisterChangedAction(UpdateValue);
    }

    public bool HasPermissions(Player player) => player.HasPermissions("frikanutils.audio");

    private void UpdateValue(Player player, float value)
    {
        if (!HasPermissions(player))
        {
            return;
        }

        AudioPlugin.Instance.Config.Volume = value;
        AudioPlugin.Instance.SaveConfig();
    }
}