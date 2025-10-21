using System;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Helpers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// Extension of the <see cref="Dropdown"/>, used to have values of a specific type.
/// </summary>
/// <typeparam name="T">The type used in the dropdown</typeparam>
public class TypedDropdown<T> : Dropdown
{
    /// <summary>
    /// The typed version of the value.
    /// </summary>
    public T TypedValue
    {
        get => _internalOptions[Setting.SyncSelectionIndexValidated];
        set
        {
            var index = _internalOptions.IndexOf(value);
            if (index >= 0)
            {
                Setting.SendValueUpdate(index, true, UpdateFilter);
            }
        }
    }

    /// <summary>
    /// The typed options for the setting.
    /// </summary>
    public T[] TypedOptions
    {
        get => _internalOptions;
        set
        {
            _internalOptions = value;
            Setting.SendDropdownUpdate(value.Select(x => _toString(x)).ToArray(), true, UpdateFilter);
        }
    }

    private T[] _internalOptions;
    private readonly Func<T, string> _toString;

    /// <summary>
    /// Create a new typed dropdown setting with the given settings.
    /// A custom `toString` method can be given, otherwise it uses the default <see cref="Object.ToString"/> method.
    /// </summary>
    /// <param name="id">Optional ID that can be used to refer to this setting</param>
    /// <param name="label">Label shown for this setting</param>
    /// <param name="options">Dropdown options</param>
    /// <param name="defaultOptionIndex">Index that is selected by default</param>
    /// <param name="entryType">The type of dropdown to show</param>
    /// <param name="hint">Additional explanation for the button</param>
    /// <param name="isServerOnly">The way the value for the setting should be stored</param>
    /// <param name="toString">The function used to convert the given type to string</param>
    public TypedDropdown(
        ushort? id,
        string label,
        T[] options,
        int defaultOptionIndex = 0,
        SSDropdownSetting.DropdownEntryType entryType = SSDropdownSetting.DropdownEntryType.Regular,
        string hint = null,
        ServerOnlyType isServerOnly = ServerOnlyType.Client,
        Func<T, string> toString = null)
        :
        base(id, label, options.Select(x => toString == null ? x.ToString() : toString(x)).ToArray(),
            defaultOptionIndex, entryType, hint, isServerOnly)
    {
        _internalOptions = options;
        _toString = toString ?? (x => x.ToString());
    }
}