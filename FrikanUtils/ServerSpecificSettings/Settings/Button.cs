using System;
using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class Button : SettingsBase
{
    public override ServerSpecificSettingBase Base => Setting;
    public readonly SSButton Setting;

    internal Action<Player> OnClick;

    public Button(
        ushort? id,
        string label,
        string buttonText,
        float? holdTimeSeconds = null,
        string hint = null) : base(id)
    {
        Setting = new SSButton(
            null,
            label,
            buttonText,
            holdTimeSeconds,
            hint
        );
    }

    public void Update(string newText = null, float? newHoldTime = null, bool applyOverride = true)
    {
        Setting.SendButtonUpdate(newText ?? Setting.ButtonText, newHoldTime ?? Setting.HoldTimeSeconds, applyOverride);
    }

    public Button RegisterClickAction(Action<Player> clickedAction)
    {
        OnClick = clickedAction;
        return this;
    }

    public override SettingsBase Clone()
    {
        return new Button(SettingId, Label, Setting.ButtonText, Setting.HoldTimeSeconds, HintDescription)
            .RegisterClickAction(OnClick);
    }
}