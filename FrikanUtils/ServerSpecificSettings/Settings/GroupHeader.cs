using FrikanUtils.ServerSpecificSettings.Helpers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class GroupHeader : SettingsBase
{
    public override ServerSpecificSettingBase Base { get; }

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