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

    public override IEnumerable<IServerSpecificSetting> GetSettings(Player player)
    {
        for (ushort i = 0; i < GlobalSettingsHandler.ServerSettings.Count; i++)
        {
            var setting = GlobalSettingsHandler.ServerSettings[i];
            if (setting.HasPermissions(player))
            {
                yield return setting.Get(i);
            }
        }
    }
}