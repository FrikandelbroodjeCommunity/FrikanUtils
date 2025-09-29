using System;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Helpers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class TypedDropdown<T> : Dropdown
{
    public T TypedValue
    {
        get => _internalOptions[Setting.SyncSelectionIndexValidated];
        set
        {
            var index = _internalOptions.IndexOf(value);
            if (index >= 0)
            {
                Setting.SendValueUpdate(index);
            }
        }
    }

    public T[] TypedOptions
    {
        get => _internalOptions;
        set
        {
            _internalOptions = value;
            Setting.SendDropdownUpdate(value.Select(x => _toString(x)).ToArray());
        }
    }

    private T[] _internalOptions;
    private readonly Func<T, string> _toString;

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