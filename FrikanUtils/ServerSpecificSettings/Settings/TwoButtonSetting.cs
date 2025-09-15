using System;
using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class TwoButtonSetting : ValueSettingsBase<bool>
{
    public override ServerSpecificSettingBase Base => Setting;
    public readonly SSTwoButtonsSetting Setting;

    public override bool Value
    {
        get => Setting.SyncIsB;
        set => Setting.SendValueUpdate(value);
    }

    public TwoButtonSetting(
        string id,
        string label,
        string optionA,
        string optionB,
        bool defaultIsB = false,
        string hint = null,
        ServerOnlyType isServerOnly = ServerOnlyType.Client) : base(id, isServerOnly)
    {
        Setting = new SSTwoButtonsSetting(
            null,
            label,
            optionA,
            optionB,
            defaultIsB,
            hint,
            isServerOnly: isServerOnly.IsServerOnly()
        );
    }

    public void Update(string optionA = null, string optionB = null, bool applyOverride = true)
    {
        Setting.SendTwoButtonUpdate(optionA ?? Setting.OptionA, optionB ?? Setting.OptionB, applyOverride);
    }

    public override SettingsBase Clone()
    {
        return new TwoButtonSetting(SettingId, Label, Setting.OptionA, Setting.OptionB, Setting.DefaultIsB,
                HintDescription, ServerOnlyType)
            .RegisterChangedAction(OnChanged);
    }
}