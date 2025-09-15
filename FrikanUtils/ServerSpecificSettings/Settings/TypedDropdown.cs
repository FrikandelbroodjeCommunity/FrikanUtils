using System.Linq;
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
            Setting.SendDropdownUpdate(value.Select(x => x.ToString()).ToArray());
        }
    }

    private T[] _internalOptions;

    public TypedDropdown(
        string id,
        string label,
        T[] options,
        int defaultOptionIndex = 0,
        SSDropdownSetting.DropdownEntryType entryType = SSDropdownSetting.DropdownEntryType.Regular,
        string hint = null,
        bool isServerOnly = false) : base(id, label, options.Select(x => x.ToString()).ToArray(), defaultOptionIndex,
        entryType, hint, isServerOnly)
    {
        _internalOptions = options;
    }

    public override SettingsBase Clone()
    {
        return new TypedDropdown<T>(SettingId, Label, _internalOptions, Setting.DefaultOptionIndex, Setting.EntryType,
                HintDescription, Setting.IsServerOnly)
            .RegisterChangedAction(OnChanged)
            .RegisterIntialValueAction(OnInitialValue);
    }
}