using FrikanUtils.ServerSpecificSettings.Helpers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// Group header for Server Specific Settings.
/// </summary>
public class GroupHeader : SettingsBase
{
    /// <inheritdoc />
    public override ServerSpecificSettingBase Base { get; }

    /// <summary>
    /// Create a new group header with the given settings.
    /// Group headers are always <see cref="ServerOnlyType.ServerOnly"/>.
    /// </summary>
    /// <param name="id">Optional ID that can be used to refer to this setting</param>
    /// <param name="label">Label shown for this setting</param>
    /// <param name="reducedPadding">Whether to reduce the padding on the header</param>
    /// <param name="hint">Additional explanation for the button</param>
    public GroupHeader(
        ushort? id,
        string label,
        bool reducedPadding = false,
        string hint = null) : base(id, ServerOnlyType.ServerOnly)
    {
        Base = new SSGroupHeader(
            label,
            reducedPadding,
            hint
        );
    }
}