using TMPro;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class TextArea : SettingsBase
{
    public override ServerSpecificSettingBase Base { get; }

    public TextArea(
        string id,
        string content,
        SSTextArea.FoldoutMode foldoutMode = SSTextArea.FoldoutMode.NotCollapsable,
        string collapsedText = null,
        TextAlignmentOptions textAlignment = TextAlignmentOptions.TopLeft) : base(id)
    {
        Base = new SSTextArea(
            null,
            content,
            foldoutMode,
            collapsedText,
            textAlignment
        );
    }
}