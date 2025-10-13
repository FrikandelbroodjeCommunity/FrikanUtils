using FrikanUtils.ServerSpecificSettings.Helpers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// Dropdown for Server Specific Settings.
/// </summary>
public class Dropdown : ValueSettingsBase<string>
{
    /// <inheritdoc />
    public override ServerSpecificSettingBase Base => Setting;

    /// <summary>
    /// The dropdown setting used by the base game.
    /// </summary>
    public readonly SSDropdownSetting Setting;

    /// <inheritdoc />
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

    /// <summary>
    /// The index of the value that is selected.
    /// </summary>
    public int SelectedIndex
    {
        get => Setting.SyncSelectionIndexValidated;
        set => Setting.SendValueUpdate(value);
    }

    /// <summary>
    /// All options visible to the player.
    /// </summary>
    public string[] Options
    {
        get => Setting.Options;
        set => Setting.SendDropdownUpdate(value);
    }


    /// <summary>
    /// Create a new dropdown with the given settings.
    /// </summary>
    /// <param name="id">Optional ID that can be used to refer to this setting</param>
    /// <param name="label">Label shown for this setting</param>
    /// <param name="options">Dropdown options</param>
    /// <param name="defaultOptionIndex">Index that is selected by default</param>
    /// <param name="entryType">The type of dropdown to show</param>
    /// <param name="hint">Additional explanation for the button</param>
    /// <param name="isServerOnly">The way the value for the setting should be stored</param>
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

    /// <inheritdoc />
    public override void CopyValue(SettingsBase setting)
    {
        base.CopyValue(setting);
        if (setting is Dropdown dropdown)
        {
            Setting.SyncSelectionIndexRaw = dropdown.Setting.SyncSelectionIndexRaw;
        }
    }
}