using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Menus;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

public class GlobalClientSettingsMenu : MenuBase
{
    public override string MenuId => "General Settings";
    public override MenuType Type => MenuType.Static;

    public override IEnumerable<SettingsBase> GetSettings(Player player)
    {
        return GlobalSettingsHandler.ClientSettings
            .Where(x => x.HasPermissions(player))
            .Select(setting => setting.Get());
    }
}