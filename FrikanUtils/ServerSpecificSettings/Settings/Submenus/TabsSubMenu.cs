using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Helpers;
using LabApi.Features.Wrappers;
using TMPro;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings.Submenus;

/// <summary>
/// A menu which shows the submenu selected by the player out of multiple submenus.
/// </summary>
public abstract class TabsSubMenu : SubMenu
{
    /// <summary>
    /// Label to show on the menu selector.
    /// </summary>
    protected abstract string Label { get; }

    /// <summary>
    /// The index of the menu that is selected by default.
    /// </summary>
    protected abstract int DefaultIndex { get; }

    /// <summary>
    /// The <see cref="ServerOnlyType"/> for the tab dropdown setting.
    /// </summary>
    protected abstract ServerOnlyType DropdownServerType { get; }

    /// <summary>
    /// The way the dropdown should be shown. <seealso cref="Dropdown"/>
    /// </summary>
    protected virtual SSDropdownSetting.DropdownEntryType DropdownType =>
        SSDropdownSetting.DropdownEntryType.Scrollable;

    private readonly ushort _settingId;

    /// <summary>
    /// The new tab sub menu, the ID the dropdown setting should be given.
    /// </summary>
    /// <param name="settingId">Free ID for the dropdown</param>
    protected TabsSubMenu(ushort settingId)
    {
        _settingId = settingId;
    }

    /// <summary>
    /// Get all available submenus for a player. These are the menus the player can choose from.
    /// The ID of the dropdown is given, so fields within the menus can start at the given ID.
    /// 
    /// <code>ToString</code> will be used to get the text displayed for the given submenu.
    /// </summary>
    /// <param name="player">The player the submenus need to be for</param>
    /// <param name="settingId">The id of the dropdown</param>
    /// <returns>All menus the player can choose from</returns>
    protected abstract IEnumerable<SubMenu> GetSubMenus(Player player, ushort settingId);

    /// <inheritdoc />
    public override IEnumerable<IServerSpecificSetting> GetSettings(Player player)
    {
        var subMenus = GetSubMenus(player, _settingId).ToArray();

        yield return new TypedDropdown<SubMenu>(_settingId, Label, subMenus, DefaultIndex, DropdownType,
                isServerOnly: DropdownServerType, toString: MenuToString)
            .RegisterChangedAction(SelectionUpdated);

        var menu = subMenus[DefaultIndex];
        if (SSSHandler.TryGetField(player, OwnerMenu, _settingId, out TypedDropdown<SubMenu> renderedSetting))
        {
            var value = renderedSetting.TypedValue;
            if (value == null)
            {
                yield return new TextArea(null,
                    "Something went wrong while determining which menu you have selected." +
                    "\n\n<i>Please select your desired option again.</i>",
                    textAlignment: TextAlignmentOptions.Center);
                yield break;
            }

            menu = value;
        }

        // Render the settings from the menu
        foreach (var setting in menu.GetSettings(player))
        {
            yield return setting;
        }
    }

    private void SelectionUpdated(Player player, string oldValue, string collapsed)
    {
        if (oldValue != collapsed)
        {
            SSSHandler.UpdatePlayer(player, OwnerMenu, false);
        }
    }

    private string MenuToString(SubMenu menu)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        return menu is INamedSubmenu namedMenu ? namedMenu.Name : menu.ToString();
    }
}