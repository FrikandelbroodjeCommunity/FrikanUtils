using System;
using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public abstract class ValueSettingsBase<T> : SettingsBase
{
    public abstract T Value { get; set; }

    internal bool ReceivedInitialValue;
    internal bool GlobalSetting;

    protected Action<Player, T, T> OnChanged;
    protected Action<Player, T> OnInitialValue;

    protected ValueSettingsBase(ushort? settingId, ServerOnlyType isServerOnly) : base(settingId, isServerOnly)
    {
        GlobalSetting = isServerOnly.IsGlobalSetting();
    }

    public ValueSettingsBase<T> RegisterChangedAction(Action<Player, T, T> changedAction)
    {
        OnChanged = changedAction;
        return this;
    }

    public ValueSettingsBase<T> RegisterInitialValueAction(Action<Player, T> intialValueAction)
    {
        OnInitialValue = intialValueAction;
        return this;
    }

    internal void OnValueChanged(Player player, T oldValue, T value)
    {
        if (!ReceivedInitialValue)
        {
            ReceivedInitialValue = true;
            OnInitialValue?.Invoke(player, value);
        }

        OnChanged?.Invoke(player, oldValue, value);

        // For global settings, set the value of all instances
        // We do however require the id as we otherwise cannot find the other fields
        if (!GlobalSetting || !SettingId.HasValue) return;
        foreach (var setting in SSSHandler.GetAllFields<ValueSettingsBase<T>>(MenuOwner, SettingId.Value))
        {
            setting.Value = value;
        }
    }
}