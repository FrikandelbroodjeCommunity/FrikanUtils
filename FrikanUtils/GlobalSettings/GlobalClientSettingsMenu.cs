using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Menus;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

public class GlobalClientSettingsMenu : MenuBase
{
    public override string Name => ClientId;
    public override MenuType Type => MenuType.Static;
    public override int Priority => MenuPriority.Highest;

    public const string ClientId = "General Settings";

    public override bool HasPermission(Player player)
    {
        return base.HasPermission(player) && GlobalSettingsHandler.ClientSettings.Any(x => x.HasPermissions(player));
    }

    public override IEnumerable<SettingsBase> GetSettings(Player player)
    {
        byte counter = 0;
        return GlobalSettingsHandler.ClientSettings
            .Where(x => x.HasPermissions(player))
            .OrderBy(x => x.Label)
            .Select(setting => setting.Get(counter++));
    }
}