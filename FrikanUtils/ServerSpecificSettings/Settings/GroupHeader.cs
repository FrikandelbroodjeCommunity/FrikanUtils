using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public class GroupHeader : SettingsBase
{
    public override ServerSpecificSettingBase Base { get; }

    public GroupHeader(
        ushort? id,
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

    public override SettingsBase Clone()
    {
        return new GroupHeader(SettingId, Label, ((SSGroupHeader)Base).ReducedPadding, HintDescription);
    }
}