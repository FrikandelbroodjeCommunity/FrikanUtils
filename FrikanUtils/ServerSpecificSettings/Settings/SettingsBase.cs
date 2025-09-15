using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public abstract class SettingsBase
{
    public byte? SettingId { get; }
    public string MenuOwner { get; internal set; }

    public abstract ServerSpecificSettingBase Base { get; }

    public string Label
    {
        get => Base.Label;
        set => Base.SendLabelUpdate(value);
    }

    public string HintDescription
    {
        get => Base.HintDescription;
        set => Base.SendHintUpdate(value);
    }

    internal Player Player;

    internal int Id
    {
        get => Base.SettingId;
        set => Base.SettingId = value;
    }

    protected SettingsBase(byte? settingId)
    {
        SettingId = settingId;
    }

    public void UpdateBase(string newLabel, string newHintDescription, bool applyOverride = true)
    {
        Base.SendUpdate(newLabel, newHintDescription, applyOverride);
    }

    public abstract SettingsBase Clone();
}