using FrikanUtils.ServerSpecificSettings.Helpers;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

internal class SpeakerVolumeSetting : IGlobalSetting
{
    public string Label => "Music volume (Speaker)";
    public bool ServerOnly => true;

    public SettingsBase Get(ushort settingId)
    {
        return new Slider(
            settingId,
            Label,
            0,
            200,
            AudioPlugin.Instance.Config.VolumeSpeaker,
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

        AudioPlugin.Instance.Config.VolumeSpeaker = value;
        AudioPlugin.Instance.SaveConfig();
    }
}