using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class GroupHeader : SettingsBase
{
    public override ServerSpecificSettingBase Base { get; }

    public GroupHeader(
        string id,
        string label,
        bool reducedPadding = false,
        string hint = null) : base(id)
    {
        Base = new SSGroupHeader(
            label,
            reducedPadding,
            hint
        );
    }
}