using System;
using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// Two button setting for Server Specific Settings.
/// </summary>
public class TwoButtonSetting : ValueSettingsBase<bool>
{
    /// <inheritdoc />
    public override ServerSpecificSettingBase Base => Setting;

    /// <summary>
    /// The two button setting used by the base game.
    /// </summary>
    public readonly SSTwoButtonsSetting Setting;

    /// <inheritdoc />
    public override bool Value
    {
        get => Setting.SyncIsB;
        set => Setting.SendValueUpdate(value, true, UpdateFilter);
    }

    /// <summary>
    /// The text shown on the button for option A.
    /// </summary>
    public string OptionA
    {
        get => Setting.OptionA;
        set => Update(value);
    }

    /// <summary>
    /// The text shown on the button for option B.
    /// </summary>
    public string OptionB
    {
        get => Setting.OptionB;
        set => Update(optionB: value);
    }

    /// <summary>
    /// Create a new two button setting with the given settings.
    /// </summary>
    /// <param name="id">Optional ID that can be used to refer to this setting</param>
    /// <param name="label">Label shown for this setting</param>
    /// <param name="optionA">Text shown on the option A button</param>
    /// <param name="optionB">Text shown on the option B button</param>
    /// <param name="defaultIsB">Whether option B is the option selected by default</param>
    /// <param name="hint">Additional explanation for the slider</param>
    /// <param name="isServerOnly">The way the value for the setting should be stored</param>
    public TwoButtonSetting(
        ushort? id,
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

    /// <summary>
    /// Update multiple values of the two button setting at the same time.
    /// </summary>
    /// <param name="optionA">The new text shown on option A</param>
    /// <param name="optionB">The new text shown on option B</param>
    /// <param name="applyOverride">Whether to apply the change immediately</param>
    public void Update(string optionA = null, string optionB = null, bool applyOverride = true)
    {
        Setting.SendTwoButtonUpdate(optionA ?? Setting.OptionA, optionB ?? Setting.OptionB, applyOverride,
            UpdateFilter);
    }

    /// <inheritdoc />
    public override void CopyValue(SettingsBase setting)
    {
        base.CopyValue(setting);
        if (setting is TwoButtonSetting twoButtonSetting)
        {
            Setting.SyncIsB = twoButtonSetting.Setting.SyncIsB;
        }
    }
}