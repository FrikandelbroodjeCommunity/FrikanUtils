using FrikanUtils.ServerSpecificSettings.Helpers;
using TMPro;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// Text area for Server Specific Settings.
/// </summary>
public class TextArea : SettingsBase
{
    /// <inheritdoc />
    public override ServerSpecificSettingBase Base { get; }

    /// <summary>
    /// Create a new button with the given settings.
    /// Buttons are always <see cref="ServerOnlyType.ServerOnly"/>.
    /// </summary>
    /// <param name="id">Optional ID that can be used to refer to this setting</param>
    /// <param name="content">Content shown for this setting</param>
    /// <param name="foldoutMode">The way the text area can be collapsed/folded out</param>
    /// <param name="collapsedText">The text shown when the area is collapsed</param>
    /// <param name="textAlignment">The way the text is aligned</param>
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
}