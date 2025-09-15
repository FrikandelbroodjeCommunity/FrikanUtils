using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Menus;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

public class GlobalServerSettingsMenu : MenuBase
{
    public override string MenuId => "General Server Settings";
    public override MenuType Type => MenuType.Dynamic;

    public override bool HasPermission(Player player)
    {
        return base.HasPermission(player) && player.RemoteAdminAccess;
    }

    public override IEnumerable<SettingsBase> GetSettings(Player player)
    {
        return GlobalSettingsHandler.ServerSettings
            .Where(x => x.HasPermissions(player))
            .Select(setting => setting.Get());
    }
}