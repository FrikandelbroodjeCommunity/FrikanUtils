using System.Collections.Generic;
using FrikanUtils.ServerSpecificSettings.Menus;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Settings.Submenus;

/// <summary>
/// Represents a sub-menu, used to group settings.
/// Can be returned as a normal setting for other <see cref="SubMenu"/>s or <see cref="MenuBase"/>.
/// </summary>
public abstract class SubMenu : IServerSpecificSetting
{
    /// <summary>
    /// The parent menu that owns this submenu instance.
    /// </summary>
    public MenuBase OwnerMenu { get; private set; }

    /// <summary>
    /// Check whether a player the required permissions to view the contents of this submenu
    /// </summary>
    /// <param name="player">The player attempting to view the menu</param>
    /// <returns>Whether the player is allowed to see the menu</returns>
    protected virtual bool HasPermission(Player player) => true;

    /// <summary>
    /// Get the settings that should be displayed for this player.
    /// </summary>
    /// <param name="player">Player to load the settings for</param>
    /// <returns>Returns a list of settings to show</returns>
    public abstract IEnumerable<IServerSpecificSetting> GetSettings(Player player);

    /// <summary>
    /// Render the contents of the menu, this only gets called when the player has permission, so no additional check is needed.
    /// </summary>
    /// <param name="menu">Parent menu this is being rendered for</param>
    /// <param name="playerMenu">Player menu that needs to be rendered to</param>
    protected virtual void RenderContents(MenuBase menu, PlayerMenu playerMenu)
    {
        foreach (var setting in GetSettings(playerMenu.TargetPlayer))
        {
            setting.RenderForMenu(menu, playerMenu);
        }
    }

    /// <inheritdoc />
    public void RenderForMenu(MenuBase menu, PlayerMenu playerMenu)
    {
        OwnerMenu = menu;

        if (InternalHasPermission(playerMenu.TargetPlayer))
        {
            RenderContents(menu, playerMenu);
        }
    }

    private bool InternalHasPermission(Player player)
    {
        try
        {
            return HasPermission(player);
        }
        catch
        {
            return false;
        }
    }
}