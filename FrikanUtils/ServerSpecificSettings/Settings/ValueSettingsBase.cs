using System;
using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// Represents a field in the Server Specific Settings that syncs a value between the server and client.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ValueSettingsBase<T> : SettingsBase
{
    /// <summary>
    /// The current value of the field, set by the client.
    /// </summary>
    public abstract T Value { get; set; }

    internal bool ReceivedInitialValue;
    internal readonly bool GlobalSetting;

    private Action<Player, T, T> _onChanged;
    private Action<Player, T> _onInitialValue;

    /// <summary>
    /// Create the base for a value.
    /// </summary>
    /// <param name="settingId">ID of the setting</param>
    /// <param name="isServerOnly">Server Only mode</param>
    protected ValueSettingsBase(ushort? settingId, ServerOnlyType isServerOnly) : base(settingId, isServerOnly)
    {
        GlobalSetting = isServerOnly == ServerOnlyType.GlobalServerOnly;
    }

    /// <summary>
    /// Register action that needs to be executed when the value is changed.
    /// The action should take 3 parameters:
    ///  - The player that changed the value
    ///  - The previous value
    ///  - The new value
    /// </summary>
    /// <param name="changedAction">Action executed on change</param>
    /// <returns>The instance, to chain operations</returns>
    public ValueSettingsBase<T> RegisterChangedAction(Action<Player, T, T> changedAction)
    {
        _onChanged = changedAction;
        return this;
    }

    /// <summary>
    /// Register action that needs to be executed when the value is changed for the first time.
    /// The action should take 2 parameters:
    ///  - The player that changed the value
    ///  - The new value
    /// </summary>
    /// <param name="intialValueAction">Action executed on initial change</param>
    /// <returns>The instance, to chain operations</returns>
    public ValueSettingsBase<T> RegisterInitialValueAction(Action<Player, T> intialValueAction)
    {
        _onInitialValue = intialValueAction;
        return this;
    }

    internal void OnValueChanged(Player player, T oldValue, T value)
    {
        if (!ReceivedInitialValue)
        {
            ReceivedInitialValue = true;
            _onInitialValue?.Invoke(player, value);
        }

        _onChanged?.Invoke(player, oldValue, value);

        // For global settings, set the value of all instances
        // We do however require the id as we otherwise cannot find the other fields
        if (!GlobalSetting || !SettingId.HasValue) return;
        foreach (var setting in SSSHandler.GetAllFields<ValueSettingsBase<T>>(MenuOwner, SettingId.Value))
        {
            setting.Value = value;
        }
    }
}