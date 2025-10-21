using FrikanUtils.ServerSpecificSettings.Helpers;
using TMPro;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// Text input for Server Specific Settings.
/// </summary>
public class TextInput : ValueSettingsBase<string>
{
    /// <inheritdoc />
    public override ServerSpecificSettingBase Base => Setting;

    /// <summary>
    /// The text input setting used by the base game.
    /// </summary>
    public readonly SSPlaintextSetting Setting;

    /// <inheritdoc />
    public override string Value
    {
        get => Setting.SyncInputText;
        set => Setting.SendValueUpdate(value, true, UpdateFilter);
    }

    /// <summary>
    /// The placeholder for the input setting.
    /// </summary>
    public string PlaceHolder
    {
        get => Setting.Placeholder;
        set => Update(value);
    }

    /// <summary>
    /// The character limit for the input. Not strictly enforced.
    /// </summary>
    public ushort CharacterLimit
    {
        get => (ushort)Setting.CharacterLimit;
        set => Update(characterLimit: value);
    }

    /// <summary>
    /// The type of content expected for the input. Not strictly enforced.
    /// </summary>
    public TMP_InputField.ContentType ContentType
    {
        get => Setting.ContentType;
        set => Update(contentType: value);
    }

    /// <summary>
    /// Create a new dropdown with the given settings.
    /// </summary>
    /// <param name="id">Optional ID that can be used to refer to this setting</param>
    /// <param name="label">Label shown for this setting</param>
    /// <param name="placeholder">The placeholder shown when there is no text</param>
    /// <param name="characterLimit">The maximum amount of characters that can be input</param>
    /// <param name="contentType">The type of content required</param>
    /// <param name="hint">Additional explanation for the slider</param>
    /// <param name="isServerOnly">The way the value for the setting should be stored</param>
    public TextInput(
        ushort? id,
        string label,
        string placeholder = "...",
        ushort characterLimit = 64,
        TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard,
        string hint = null,
        ServerOnlyType isServerOnly = ServerOnlyType.Client) : base(id, isServerOnly)
    {
        Setting = new SSPlaintextSetting(
            null,
            label,
            placeholder,
            characterLimit,
            contentType,
            hint,
            isServerOnly: isServerOnly.IsServerOnly()
        );
    }

    /// <summary>
    /// Update multiple values of the text input at once.
    /// </summary>
    /// <param name="placeholder">The new placeholder for the input</param>
    /// <param name="characterLimit">The new charachter limit for the input</param>
    /// <param name="contentType">The new content type for the input</param>
    /// <param name="applyOverride">Whether to apply the change immediately</param>
    public void Update(
        string placeholder = null,
        ushort? characterLimit = null,
        TMP_InputField.ContentType? contentType = null,
        bool applyOverride = true)
    {
        Setting.SendPlaintextUpdate(
            placeholder ?? Setting.Placeholder,
            characterLimit ?? (ushort)Setting.CharacterLimit,
            contentType ?? Setting.ContentType,
            applyOverride,
            UpdateFilter
        );
    }

    /// <summary>
    /// Clear the contents of the text input.
    /// </summary>
    public void Clear()
    {
        ((SSPlaintextSetting)Base).SyncInputText = "";
        ((SSPlaintextSetting)Base).SendClearRequest(UpdateFilter);
    }

    /// <inheritdoc />
    public override void CopyValue(SettingsBase setting)
    {
        base.CopyValue(setting);
        if (setting is TextInput text)
        {
            Setting.SyncInputText = text.Setting.SyncInputText;
        }
    }
}