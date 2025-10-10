namespace FrikanUtils.ServerSpecificSettings.Settings.Submenus;

/// <summary>
/// Tells the menu system this submenu has a name.
/// This can be used for <see cref="TabsSubMenu"/>s to display the name properly instead of using <code>ToString</code>.
/// </summary>
public interface INamedSubmenu
{
    /// <summary>
    /// Name that should be displayed for this menu.
    /// </summary>
    public string Name { get; }
}