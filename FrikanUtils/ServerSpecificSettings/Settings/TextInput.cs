using FrikanUtils.ServerSpecificSettings.Helpers;
using TMPro;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class TextInput : ValueSettingsBase<string>
{
    public override ServerSpecificSettingBase Base => Setting;
    public readonly SSPlaintextSetting Setting;

    public override string Value
    {
        get => Setting.SyncInputText;
        set => Setting.SendValueUpdate(value);
    }

    public TextInput(
        string id,
        string label,
        string placeholder = "...",
        int characterLimit = 64,
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
            applyOverride
        );
    }

    public void Clear()
    {
        ((SSPlaintextSetting)Base).SyncInputText = "";
        ((SSPlaintextSetting)Base).SendClearRequest(x => x == Player.ReferenceHub);
    }

    public override SettingsBase Clone()
    {
        return new TextInput(SettingId, Label, Setting.Placeholder, Setting.CharacterLimit, Setting.ContentType,
                HintDescription, ServerOnlyType)
            .RegisterChangedAction(OnChanged)
            .RegisterIntialValueAction(OnInitialValue);
    }
}