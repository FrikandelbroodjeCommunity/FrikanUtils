using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Menus;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

public class GlobalServerSettingsMenu : MenuBase
{
    public override string Name => ServerId;
    public override MenuType Type => MenuType.Dynamic;
    public override int Priority => MenuPriority.Highest;

    public const string ServerId = "General Server Settings";
    
    public override bool HasPermission(Player player)
    {
        return base.HasPermission(player) && GlobalSettingsHandler.ServerSettings.Any(x => x.HasPermissions(player));
    }

    public override IEnumerable<SettingsBase> GetSettings(Player player)
    {
        byte counter = 0;
        return GlobalSettingsHandler.ServerSettings
            .Where(x => x.HasPermissions(player))
            .OrderBy(x => x.Label)
            .Select(setting => setting.Get(counter++));
    }
}