using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class Dropdown : ValueSettingsBase<string>
{
    public override ServerSpecificSettingBase Base => Setting;
    public readonly SSDropdownSetting Setting;

    public string Value
    {
        get => Setting.SyncSelectionText;
        set
        {
            var index = Setting.Options.IndexOf(value);
            if (index >= 0)
            {
                Setting.SendValueUpdate(index);
            }
        }
    }

    public int SelectedIndex
    {
        get => Setting.SyncSelectionIndexValidated;
        set => Setting.SendValueUpdate(value);
    }

    public string[] Options
    {
        get => Setting.Options;
        set => Setting.SendDropdownUpdate(value);
    }

    public Dropdown(
        string id,
        string label,
        string[] options,
        int defaultOptionIndex = 0,
        SSDropdownSetting.DropdownEntryType entryType = SSDropdownSetting.DropdownEntryType.Regular,
        string hint = null,
        bool isServerOnly = false) : base(id)
    {
        Setting = new SSDropdownSetting(
            null,
            label,
            options,
            defaultOptionIndex,
            entryType,
            hint,
            isServerOnly: isServerOnly
        );
    }

    public override SettingsBase Clone()
    {
        return new Dropdown(SettingId, Label, Options, Setting.DefaultOptionIndex, Setting.EntryType, HintDescription,
            Setting.IsServerOnly)
            .RegisterChangedAction(OnChanged)
            .RegisterIntialValueAction(OnInitialValue);
    }
}