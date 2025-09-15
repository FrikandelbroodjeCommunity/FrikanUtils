using System;
using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using TMPro;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Menus;

public class PlayerMenu
{
    internal SSDropdownSetting MenuSelection;

    private bool _isOpen;
    private bool _isDirty;
    private readonly Player _targetPlayer;

    private MenuBase _selectedDynamicMenu;
    private readonly List<MenuBase> _shownMenus = [];
    private readonly List<SettingsBase> _shownItems = [];
    private readonly List<ServerSpecificSettingBase> _rendering = [];
    private readonly PlayerIdHandler _idHandler = new();

    private readonly List<string> _selectorOptions = [];

    public PlayerMenu(Player user)
    {
        _targetPlayer = user;
    }

    /// <summary>
    /// Get the setting belonging to this player.
    /// If the setting was not found, or the type does not match, it will return null.
    /// </summary>
    /// <param name="menu">The exact name of the menu</param>
    /// <param name="settingId">ID of the setting</param>
    /// <typeparam name="T">Type of the setting</typeparam>
    /// <returns>The found setting or null</returns>
    public T GetSetting<T>(string menu, byte settingId) where T : SettingsBase
    {
        var found = _shownItems.FirstOrDefault(x => x.MenuOwner == menu && x.SettingId == settingId);
        return found as T;
    }

    /// <summary>
    /// Update the settings shown for this player.
    /// </summary>
    public void Update(bool force = false)
    {
        if (!force && !_isOpen)
        {
            _isDirty = true;
        }

        _isDirty = false;
        _shownItems.Clear();
        _shownMenus.Clear();

        Logger.Debug($"Updating player {_targetPlayer.LogName}", UtilitiesPlugin.PluginConfig.Debug);
        if (SSSHandler.StaticMenus.Count == 0 && SSSHandler.DynamicMenus.Count == 0)
        {
            ServerSpecificSettingsSync.SendToPlayer(_targetPlayer.ReferenceHub, []);
            return;
        }

        _idHandler.Reset();
        RenderForcedDynamicMenus();
        RenderDynamicMenus();
        RenderStaticMenus();

        ServerSpecificSettingsSync.SendToPlayer(_targetPlayer.ReferenceHub, _rendering.ToArray());
        _rendering.Clear(); // Clear after, as we don't need references anymore
    }

    /// <summary>
    /// Try to update the menu based on the visibility
    /// </summary>
    /// <param name="menu">The menu that needs to be visible in order to update</param>
    /// <param name="force">Whether to force update, or wait for the player to open the menu</param>
    public void TryUpdate(MenuBase menu, bool force = false)
    {
        // If the player has access to the menu, or the menu is still shown (even though access has been removed)
        if (HasMenu(menu) || _shownMenus.Contains(menu))
        {
            Update(force);
        }
    }

    /// <summary>
    /// Check whether a player is currently shown the given menu.
    /// </summary>
    /// <param name="menu">Manu to question</param>
    /// <returns>Whether the menu is shown</returns>
    public bool HasMenu(MenuBase menu)
    {
        if (!menu.InternalHasPermission(_targetPlayer))
        {
            return false;
        }

        if (menu.Type == MenuType.Dynamic)
        {
            return _selectedDynamicMenu?.Equals(menu) ?? false;
        }

        return true;
    }

    internal void SetOpen(bool status)
    {
        _isOpen = status;

        if (status && _isDirty)
        {
            Update();
        }
    }

    internal void SwitchWindow(int index)
    {
        // If we are out of bounds, or the index is 0 (aka "No menu"), clear the menu
        if (index <= 0 || _selectorOptions.Count <= index)
        {
            if (_selectedDynamicMenu != null)
            {
                _selectedDynamicMenu = null;
                Update();
            }

            return;
        }

        var target = SSSHandler.DynamicMenus.FirstOrDefault(x => x.Name == _selectorOptions[index]);
        if (_selectedDynamicMenu == null && target == null)
        {
            return;
        }

        if (_selectedDynamicMenu != null && _selectedDynamicMenu.Equals(target))
        {
            return;
        }

        _selectedDynamicMenu = target;
        Update();
    }

    internal SettingsBase GetSetting(int settingId, Type expectedType)
    {
        var found = _shownItems.FirstOrDefault(x => x.Id == settingId);
        if (found == null || found.Base.GetType() != expectedType)
        {
            return null;
        }

        return found;
    }

    private void RenderForcedDynamicMenus()
    {
        foreach (var menu in SSSHandler.DynamicMenus
                     .Where(x => x.InternalHasPermission(_targetPlayer) && x.Type == MenuType.Forced))
        {
            _rendering.Add(new SSGroupHeader($"<size=20>{menu.Name}</size>", true));

            if (RenderMenu(menu))
            {
                _shownMenus.Add(menu);
            }
            else // If the menu has no contents, remove the header
            {
                _rendering.RemoveAt(_rendering.Count - 1);
            }
        }
    }

    private void RenderDynamicMenus()
    {
        // Get the options for the dropdown
        _selectorOptions.Clear();
        _selectorOptions.Add("No menu");
        _selectorOptions.AddRange(SSSHandler.DynamicMenus
            .Where(x => x.InternalHasPermission(_targetPlayer) && x.Type == MenuType.Dynamic).OrderBy(x => x.Name)
            .Select(x => x.Name)
        );

        // If we only have the "No menu item", don't show anything of the dynamic menu system
        if (_selectorOptions.Count <= 1)
        {
            return;
        }

        var index = 0;

        // If a menu is already selected, keep that index
        if (_selectedDynamicMenu != null && _selectorOptions.Contains(_selectedDynamicMenu.Name))
        {
            index = _selectorOptions.IndexOf(_selectedDynamicMenu.Name);
        }

        // Show the header
        _rendering.Add(new SSGroupHeader("<b>Menus</b>"));

        // Show dropdown selection
        MenuSelection = new SSDropdownSetting(-3, "Selected dynamic menu", _selectorOptions.ToArray(), index,
            hint: "Determines which menu is shown below. Select \'No menu\' to not show a menu.");
        _rendering.Add(MenuSelection);

        // Render the selected menu or empty message
        if (_selectedDynamicMenu == null || !_selectedDynamicMenu.InternalHasPermission(_targetPlayer))
        {
            _rendering.Add(new SSTextArea(-4, "Currently you have no valid menu selected!"));
        }
        else if (RenderMenu(_selectedDynamicMenu))
        {
            _shownMenus.Add(_selectedDynamicMenu);
        }
        else
        {
            _rendering.Add(new SSTextArea(-4, "The menu is currently empty."));
        }
    }

    private void RenderStaticMenus()
    {
        var hasMenu = false;

        _rendering.Add(new SSGroupHeader("<b>Settings</b>"));
        foreach (var menu in SSSHandler.StaticMenus.Where(x => x.InternalHasPermission(_targetPlayer))
                     .OrderByDescending(x => x.Priority))
        {
            _rendering.Add(new SSGroupHeader($"<size=20>{menu.Name}</size>", true));

            if (RenderMenu(menu))
            {
                hasMenu = true;
                _shownMenus.Add(menu);
            }
            else // If the menu has no contents, remove the header
            {
                _rendering.RemoveAt(_rendering.Count - 1);
            }
        }

        if (!hasMenu) // If there are no menus for this player, show a message
        {
            _rendering.Add(new SSTextArea(-2, UtilitiesPlugin.PluginConfig.NoSettingsText,
                textAlignment: TextAlignmentOptions.Top));
        }
    }

    private bool RenderMenu(MenuBase menu)
    {
        var addedItem = false;
        try
        {
            foreach (var item in menu.GetSettings(_targetPlayer))
            {
                item.Player = _targetPlayer;
                item.Id = _idHandler.GetId(menu.Name, item.SettingId);
                item.MenuOwner = menu.Name;
                _shownItems.Add(item);
                _rendering.Add(item.Base);

                addedItem = true;
            }
        }
        catch (Exception e)
        {
            Logger.Debug($"Exception while rendering menu: {menu.Name}.\n{e}", UtilitiesPlugin.PluginConfig.Debug);
        }

        return addedItem;
    }
}