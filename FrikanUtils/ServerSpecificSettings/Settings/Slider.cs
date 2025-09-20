using System;
using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class Slider : ValueSettingsBase<float>
{
    public override ServerSpecificSettingBase Base => Setting;
    public readonly SSSliderSetting Setting;

    public override float Value
    {
        get => Setting.SyncFloatValue;
        set => Setting.SendValueUpdate(value);
    }

    public int IntValue
    {
        get => Setting.SyncIntValue;
        set => Setting.SendValueUpdate(value);
    }

    internal Action<Player, int> OnChangedInt;
    internal Action<Player, int> OnInitialValueInt;

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
            applyOverride
        );
    }

    public Slider RegisterChangedIntAction(Action<Player, int> changedAction)
    {
        OnChangedInt = changedAction;
        return this;
    }

    public Slider RegisterIntialValueIntAction(Action<Player, int> intialValueAction)
    {
        OnInitialValueInt = intialValueAction;
        return this;
    }
}