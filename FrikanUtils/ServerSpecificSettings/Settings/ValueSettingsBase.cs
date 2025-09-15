using System;
using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public abstract class ValueSettingsBase<T> : SettingsBase
{
    public abstract T Value { get; set; }

    internal bool ReceivedInitialValue;
    internal bool GlobalSetting;
    internal ServerOnlyType ServerOnlyType;
    internal Action<Player, T> OnChanged;
    internal Action<Player, T> OnInitialValue;

    protected ValueSettingsBase(string settingId, ServerOnlyType isServerOnly) : base(settingId)
    {
        GlobalSetting = isServerOnly.IsGlobalSetting();
        ServerOnlyType = isServerOnly;
    }

    public ValueSettingsBase<T> RegisterChangedAction(Action<Player, T> changedAction)
    {
        OnChanged = changedAction;
        return this;
    }

    public ValueSettingsBase<T> RegisterIntialValueAction(Action<Player, T> intialValueAction)
    {
        OnInitialValue = intialValueAction;
        return this;
    }

    internal void OnValueChanged(Player player, T value)
    {
        if (!ReceivedInitialValue)
        {
            ReceivedInitialValue = true;
            OnInitialValue?.Invoke(player, value);
        }

        OnInitialValue(player, value);

        if (GlobalSetting)
        {
            Value = value;
        }
    }
}