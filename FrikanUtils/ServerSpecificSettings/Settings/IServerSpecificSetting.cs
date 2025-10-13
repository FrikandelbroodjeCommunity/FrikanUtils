using FrikanUtils.ServerSpecificSettings.Menus;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// A Server Specific Setting which can be rendered for menus.
/// </summary>
public interface IServerSpecificSetting
{
    /// <summary>
    /// Render this setting to the given <see cref="PlayerMenu"/>.
    /// Additionally, the <see cref="MenuBase"/> this setting needs to render for is given.
    ///
    /// Use the <see cref="PlayerMenu.Rendering"/> and <see cref="PlayerMenu.RenderingItems"/> to render the items.
    /// </summary>
    /// <param name="menu">The menu the setting is rendered for</param>
    /// <param name="playerMenu">The player menu the setting needs to render to</param>
    public void RenderForMenu(MenuBase menu, PlayerMenu playerMenu);
}