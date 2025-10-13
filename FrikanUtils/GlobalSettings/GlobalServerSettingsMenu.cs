using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Menus;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

/// <summary>
/// Menu for displaying the global server settings.
/// This displays the global settings that are registered with <see cref="IGlobalSetting.ServerOnly"/> set to <c>true</c>.
/// </summary>
public class GlobalServerSettingsMenu : MenuBase
{
    /// <inheritdoc />
    public override string Name => ServerId;

    /// <inheritdoc />
    public override MenuType Type => MenuType.Dynamic;

    /// <inheritdoc />
    public override int Priority => MenuPriority.Highest;

    /// <summary>
    /// The <see cref="Name"/> of this menu as a constant.
    /// </summary>
    public const string ServerId = "General Server Settings";

    /// <inheritdoc />
    public override bool HasPermission(Player player)
    {
        return base.HasPermission(player) && GlobalSettingsHandler.ServerSettings.Any(x => x.HasPermissions(player));
    }

    /// <inheritdoc />
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