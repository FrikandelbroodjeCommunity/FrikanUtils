using FrikanUtils.Audio;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

public class MuteSetting : IGlobalSetting
{
    public string Label => "Audio preference";
    public bool ServerOnly => false;

    public SettingsBase Get(byte settingId)
    {
        return new TwoButtonSetting(
            settingId,
            Label,
            "Enabled",
            "Muted",
            hint: "This is only a preference, even when muted, <b>some audio may persist!</b>"
        ).RegisterChangedAction(AudioSystem.SetUserPreference);
    }

    public bool HasPermissions(Player player) => true;
}