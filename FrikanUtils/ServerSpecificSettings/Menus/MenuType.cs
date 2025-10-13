namespace FrikanUtils.ServerSpecificSettings.Menus;

/// <summary>
/// Types of menus that can be used.
/// </summary>
public enum MenuType
{
    /// <summary>
    /// A static menu will always be loaded by players who have permissions.
    /// Fields in these menus will always be assigned the same integer ID.
    ///
    /// This is intended for storing player preferences and/or keybinds.
    /// </summary>
    Static,

    /// <summary>
    /// A dynamic menu will only load when a player has the dynamic menu selected.
    /// Fields in this menu will be assigned a random ID, thus the value cannot be reliably stored on the client.
    ///
    /// This is intended for interactive menus (e.g. creating broadcasts)
    /// </summary>
    Dynamic,

    /// <summary>
    /// A forced menu will always load on the very top of the Server Specific Settings menu.
    /// Fields in this menu will be assigned a random ID, thus the value cannot be reliably stored on the client.
    ///
    /// This is intended for interactive menus that depend on the current game state (e.g. reacting on a vote run by a plugin)
    /// </summary>
    Forced
}