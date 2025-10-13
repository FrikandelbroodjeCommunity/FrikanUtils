namespace FrikanUtils.ServerSpecificSettings.Menus;

/// <summary>
/// Some default priority values for static menus.
/// </summary>
public static class MenuPriority
{
    /// <summary>
    /// Groups the menu at the very bottom.
    /// </summary>
    public const int Last = 0;

    /// <summary>
    /// Groups the menu at the lower end.
    /// </summary>
    public const int Low = 20;

    /// <summary>
    /// Groups the menu at the center.
    /// </summary>
    public const int Normal = 40;

    /// <summary>
    /// Groups the menu at the upper end.
    /// </summary>
    public const int High = 60;

    /// <summary>
    /// Groups the menu at the very top.
    /// </summary>
    public const int Highest = 100;
}