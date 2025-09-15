using System;
using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class TwoButtonSetting : SettingsBase
{
    public override ServerSpecificSettingBase Base => Setting;
    public readonly SSTwoButtonsSetting Setting;

    public bool IsBSelected
    {
        get => Setting.SyncIsB;
        set => Setting.SendValueUpdate(value);
    }

    internal Action<Player, bool> OnChanged;

    public TwoButtonSetting(
        string id,
        string label,
        string optionA,
        string optionB,
        bool defaultIsB = false,
        string hint = null,
        bool isServerOnly = false) : base(id)
    {
        Setting = new SSTwoButtonsSetting(
            null,
            label,
            optionA,
            optionB,
            defaultIsB,
            hint,
            isServerOnly: isServerOnly
        );
    }

    public void Update(string optionA = null, string optionB = null, bool applyOverride = true)
    {
        Setting.SendTwoButtonUpdate(optionA ?? Setting.OptionA, optionB ?? Setting.OptionB, applyOverride);
    }

    public TwoButtonSetting RegisterChangedAction(Action<Player, bool> changedAction)
    {
        OnChanged = changedAction;
        return this;
    }
    
    public override SettingsBase Clone()
    {
        return new TwoButtonSetting(SettingId, Label, Setting.OptionA, Setting.OptionB, Setting.DefaultIsB,
                HintDescription, Setting.IsServerOnly)
            .RegisterChangedAction(OnChanged);
    }
}