using System;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public abstract class ValueSettingsBase<T> : SettingsBase
{
    internal bool ReceivedInitialValue;
    internal Action<Player, T> OnChanged;
    internal Action<Player, T> OnInitialValue;

    protected ValueSettingsBase(string settingId) : base(settingId)
    {
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
}