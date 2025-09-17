using FrikanUtils.ServerSpecificSettings.Helpers;
using TMPro;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class TextArea : SettingsBase
{
    public override ServerSpecificSettingBase Base { get; }

    public TextArea(
        ushort? id,
        string content,
        SSTextArea.FoldoutMode foldoutMode = SSTextArea.FoldoutMode.NotCollapsable,
        string collapsedText = null,
        TextAlignmentOptions textAlignment = TextAlignmentOptions.TopLeft) : base(id, ServerOnlyType.ServerOnly)
    {
        Base = new SSTextArea(
            null,
            content,
            foldoutMode,
            collapsedText,
            textAlignment
        );
    }

    public override SettingsBase Clone()
    {
        var area = (SSTextArea)Base;
        return new TextArea(SettingId, Label, area.Foldout, HintDescription, area.AlignmentOptions);
    }
}