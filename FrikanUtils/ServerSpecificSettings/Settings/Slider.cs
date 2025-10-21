using System;
using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// Slider for Server Specific Settings.
/// </summary>
public class Slider : ValueSettingsBase<float>
{
    /// <inheritdoc />
    public override ServerSpecificSettingBase Base => Setting;

    /// <summary>
    /// The slider setting used by the base game.
    /// </summary>
    public readonly SSSliderSetting Setting;

    /// <inheritdoc />
    public override float Value
    {
        get => Setting.SyncFloatValue;
        set => Setting.SendValueUpdate(value, true, UpdateFilter);
    }

    /// <summary>
    /// The same as <see cref="Value"/>, but as an integer.
    /// </summary>
    public int IntValue
    {
        get => Setting.SyncIntValue;
        set => Setting.SendValueUpdate(value, true, UpdateFilter);
    }

    /// <summary>
    /// The lowest value the slider can move to. Not strictly enforced.
    /// </summary>
    public float MinValue
    {
        get => Setting.MinValue;
        set => Update(value);
    }

    /// <summary>
    /// The highest value the slider can move to. Not strictly enforced.
    /// </summary>
    public float MaxValue
    {
        get => Setting.MaxValue;
        set => Update(max: value);
    }

    /// <summary>
    /// Whether the slider is in integer mode.
    /// </summary>
    public bool Integer
    {
        get => Setting.Integer;
        set => Update(integer: value);
    }

    /// <summary>
    /// The way the value is converted to a string
    /// </summary>
    public string ValueToStringFormat
    {
        get => Setting.ValueToStringFormat;
        set => Update(valueToStringFormat: value);
    }

    /// <summary>
    /// The way the value is displayed for the user.
    /// </summary>
    public string FinalDisplayFormat
    {
        get => Setting.FinalDisplayFormat;
        set => Update(finalDisplayFormat: value);
    }

    internal Action<Player, int, int> OnChangedInt;
    internal Action<Player, int> OnInitialValueInt;

    /// <summary>
    /// Create a new slider with the given settings.
    /// The min and max value are not enforced, users can set them outside of the range.
    /// </summary>
    /// <param name="id">Optional ID that can be used to refer to this setting</param>
    /// <param name="label">Label shown for this setting</param>
    /// <param name="minValue">The lowerst value the slider can move to</param>
    /// <param name="maxValue">The highest value the slider can move to</param>
    /// <param name="defaultValue">The default value for the setting</param>
    /// <param name="integer">Whether the slider is in integer mode</param>
    /// <param name="valueToStringFormat">Format to use when converting the value to a string</param>
    /// <param name="finalDisplayFormat">The format used to display the value to the user</param>
    /// <param name="hint">Additional explanation for the slider</param>
    /// <param name="isServerOnly">The way the value for the setting should be stored</param>
    public Slider(
        ushort? id,
        string label,
        float minValue,
        float maxValue,
        float defaultValue = 0.0f,
        bool integer = false,
        string valueToStringFormat = "0.##",
        string finalDisplayFormat = "{0}",
        string hint = null,
        ServerOnlyType isServerOnly = ServerOnlyType.Client) : base(id, isServerOnly)
    {
        Setting = new SSSliderSetting(
            null,
            label,
            minValue,
            maxValue,
            defaultValue,
            integer,
            valueToStringFormat,
            finalDisplayFormat,
            hint,
            isServerOnly: isServerOnly.IsServerOnly()
        );
    }

    /// <summary>
    /// Update the values of this setting. Allows for multiple values to be updated at the same time.
    /// </summary>
    /// <param name="min">The new minimum value</param>
    /// <param name="max">The new maximum value</param>
    /// <param name="integer">The new integer mode</param>
    /// <param name="valueToStringFormat">The new initial conversion format</param>
    /// <param name="finalDisplayFormat">The final conversion format</param>
    /// <param name="applyOverride">Whether to apply the change immediately</param>
    public void Update(
        float? min = null,
        float? max = null,
        bool? integer = null,
        string valueToStringFormat = null,
        string finalDisplayFormat = null,
        bool applyOverride = true)
    {
        Setting.SendSliderUpdate(
            min ?? Setting.MinValue,
            max ?? Setting.MaxValue,
            integer ?? Setting.Integer,
            valueToStringFormat ?? Setting.ValueToStringFormat,
            finalDisplayFormat ?? Setting.FinalDisplayFormat,
            applyOverride,
            UpdateFilter
        );
    }

    /// <summary>
    /// Register action that needs to be executed when the value is changed.
    /// This will use the integer value instead of the float value.
    /// The action should take 3 parameters:
    ///  - The player that changed the value
    ///  - The previous value
    ///  - The new value
    /// </summary>
    /// <param name="changedAction">Action executed on change</param>
    /// <returns>The instance, to chain operations</returns>
    public Slider RegisterChangedIntAction(Action<Player, int, int> changedAction)
    {
        OnChangedInt = changedAction;
        return this;
    }

    /// <summary>
    /// Register action that needs to be executed when the value is changed for the first time.
    /// This will use the integer value instead of the float value.
    /// The action should take 2 parameters:
    ///  - The player that changed the value
    ///  - The new value
    /// </summary>
    /// <param name="intialValueAction">Action executed on initial change</param>
    /// <returns>The instance, to chain operations</returns>
    public Slider RegisterIntialValueIntAction(Action<Player, int> intialValueAction)
    {
        OnInitialValueInt = intialValueAction;
        return this;
    }

    /// <inheritdoc />
    public override void CopyValue(SettingsBase setting)
    {
        base.CopyValue(setting);
        if (setting is Slider slider)
        {
            Setting.SyncFloatValue = slider.Setting.SyncFloatValue;
        }
    }
}