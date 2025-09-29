using FrikanUtils.ServerSpecificSettings.Helpers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class Dropdown : ValueSettingsBase<string>
{
    public override ServerSpecificSettingBase Base => Setting;
    public readonly SSDropdownSetting Setting;

    public override string Value
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
        ushort? id,
        string label,
        string[] options,
        int defaultOptionIndex = 0,
        SSDropdownSetting.DropdownEntryType entryType = SSDropdownSetting.DropdownEntryType.Regular,
        string hint = null,
        ServerOnlyType isServerOnly = ServerOnlyType.Client) : base(id, isServerOnly)
    {
        Setting = new SSDropdownSetting(
            null,
            label,
            options,
            defaultOptionIndex,
            entryType,
            hint,
            isServerOnly: isServerOnly != ServerOnlyType.Client
        );
    }

    public override void CopyValue(SettingsBase setting)
    {
        base.CopyValue(setting);
        if (setting is Dropdown dropdown)
        {
            Setting.SyncSelectionIndexRaw = dropdown.Setting.SyncSelectionIndexRaw;
        }
    }
}