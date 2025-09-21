using System.Collections.Generic;
using FrikanUtils.ServerSpecificSettings.Menus;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Settings.Submenus;

public abstract class SubMenu : IServerSpecificSetting
{
    /// <summary>
    /// Get the settings that should be displayed for this player.
    /// </summary>
    /// <param name="player">Player to load the settings for</param>
    /// <returns>Returns a list of settings to show</returns>
    protected abstract IEnumerable<IServerSpecificSetting> GetSettings(Player player);

    public virtual void RenderForMenu(MenuBase menu, PlayerMenu playerMenu)
    {
        foreach (var setting in GetSettings(playerMenu.TargetPlayer))
        {
            setting.RenderForMenu(menu, playerMenu);
        }
    }
}