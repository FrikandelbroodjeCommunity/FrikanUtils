using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Menus;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

/// <summary>
/// Menu for displaying the global client settings.
/// This displays the global settings that are registered with <see cref="IGlobalSetting.ServerOnly"/> set to <c>false</c>.
/// </summary>
public class GlobalClientSettingsMenu : MenuBase
{
    /// <inheritdoc />
    public override string Name => ClientId;

    /// <inheritdoc />
    public override MenuType Type => MenuType.Static;

    /// <inheritdoc />
    public override int Priority => MenuPriority.Highest;

    /// <summary>
    /// The <see cref="Name"/> of this menu as a constant.
    /// </summary>
    public const string ClientId = "General Settings";

    /// <inheritdoc />
    public override bool HasPermission(Player player)
    {
        return base.HasPermission(player) && GlobalSettingsHandler.ClientSettings.Any(x => x.HasPermissions(player));
    }

    /// <inheritdoc />
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