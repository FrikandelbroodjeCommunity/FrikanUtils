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

    public override IEnumerable<IServerSpecificSetting> GetSettings(Player player)
    {
        var updated = false;
        foreach (var setting in GlobalSettingsHandler.ClientSettings.Where(x => x.HasPermissions(player)))
        {
            var id = UtilitiesPlugin.PluginConfig.GlobalClientSettings.IndexOf(setting.Label);
            if (id < 0)
            {
                id = UtilitiesPlugin.PluginConfig.GlobalClientSettings.Count;
                UtilitiesPlugin.PluginConfig.GlobalClientSettings.Add(setting.Label);
                updated = true;
            }

            yield return setting.Get((ushort)id);
        }

        if (updated)
        {
            UtilitiesPlugin.Save();
        }
    }
}