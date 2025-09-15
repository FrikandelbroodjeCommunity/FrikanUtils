using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Menus;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

public class GlobalServerSettingsMenu : MenuBase
{
    public override string Name => "General Server Settings";
    public override MenuType Type => MenuType.Dynamic;

    public override bool HasPermission(Player player)
    {
        return base.HasPermission(player) && GlobalSettingsHandler.ServerSettings.Any(x => x.HasPermissions(player));
    }

    public override IEnumerable<SettingsBase> GetSettings(Player player)
    {
        Logger.Info("Gathering server settings");
        byte counter = 0;
        return GlobalSettingsHandler.ServerSettings
            .Where(x => x.HasPermissions(player))
            .Select(setting => setting.Get(counter++));
    }
}