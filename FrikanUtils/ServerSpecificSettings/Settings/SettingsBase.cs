using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public abstract class SettingsBase
{
    public ushort? SettingId { get; }
    public string MenuOwner { get; internal set; }
    public readonly ServerOnlyType ServerOnlyType;

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

    protected SettingsBase(ushort? settingId, ServerOnlyType serverOnlyType)
    {
        SettingId = settingId;
        ServerOnlyType = serverOnlyType;
    }

    public void UpdateBase(string newLabel, string newHintDescription, bool applyOverride = true)
    {
        Base.SendUpdate(newLabel, newHintDescription, applyOverride);
    }

    public abstract SettingsBase Clone();
}