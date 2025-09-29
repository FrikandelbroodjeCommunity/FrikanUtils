using System;
using System.Collections.Generic;
using System.Linq;
using FrikanUtils.ServerSpecificSettings.Helpers;
using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using TMPro;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Menus;

public class PlayerMenu
{
    internal SSDropdownSetting MenuSelection;

    internal readonly Player TargetPlayer;
    internal readonly List<SettingsBase> RenderingItems = [];
    internal readonly List<ServerSpecificSettingBase> Rendering = [];
    internal readonly PlayerIdHandler IDHandler = new();

    private bool _isOpen;
    private bool _isDirty;

    private MenuBase _selectedDynamicMenu;
    private readonly Dictionary<int, SettingsBase> _shownItems = new();
    private readonly List<MenuBase> _shownMenus = [];
    private readonly List<string> _selectorOptions = [];

    internal PlayerMenu(Player user)
    {
        TargetPlayer = user;
    }

    internal T GetSetting<T>(string menu, ushort settingId) where T : SettingsBase
    {
        var id = IDHandler.GetId(menu, settingId);
        return _shownItems.TryGetValue(id, out var value) ? value as T : null;
    }

    internal void Update(bool force)
    {
        if (!force && !_isOpen)
        {
            _isDirty = true;
            return;
        }

        _isDirty = false;
        _shownMenus.Clear();

        Logger.Debug($"Updating player {TargetPlayer.LogName}", UtilitiesPlugin.PluginConfig.Debug);
        if (SSSHandler.RegisteredMenus.Count == 0)
        {
            ServerSpecificSettingsSync.SendToPlayer(TargetPlayer.ReferenceHub, []);
            return;
        }

        IDHandler.Reset();
        RenderForcedDynamicMenus();
        RenderDynamicMenus();
        RenderStaticMenus();

        foreach (var item in RenderingItems)
        {
            // Keep value if it already existed
            if (_shownItems.TryGetValue(item.Id, out var setting))
            {
                item.CopyValue(setting);
            }

            _shownItems[item.Id] = item;
        }

        ServerSpecificSettingsSync.SendToPlayer(TargetPlayer.ReferenceHub, Rendering.ToArray());
        Rendering.Clear(); // Clear after, as we don't need references anymore
        RenderingItems.Clear();
    }

    internal void TryUpdate(MenuBase menu, bool force)
    {
        // If the player has access to the menu, or the menu is still shown (even though access has been removed)
        if (HasMenu(menu) || _shownMenus.Contains(menu))
        {
            Update(force);
        }
    }

    internal bool HasMenu(MenuBase menu)
    {
        if (!menu.InternalHasPermission(TargetPlayer))
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
            Update(true);
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
                Update(false);
            }

            return;
        }

        var target = SSSHandler.RegisteredMenus.FirstOrDefault(x => x.Name == _selectorOptions[index]);
        if (_selectedDynamicMenu == null && target == null)
        {
            return;
        }

        if (_selectedDynamicMenu != null && _selectedDynamicMenu.Equals(target))
        {
            return;
        }

        _selectedDynamicMenu = target;
        Update(false);
    }

    internal SettingsBase GetSetting(int settingId, Type expectedType)
    {
        if (_shownItems.TryGetValue(settingId, out var setting) && setting.Base.GetType() == expectedType)
        {
            return setting;
        }

        return null;
    }

    private void RenderForcedDynamicMenus()
    {
        foreach (var menu in SSSHandler.RegisteredMenus
                     .Where(x => x.InternalHasPermission(TargetPlayer) && x.Type == MenuType.Forced))
        {
            Rendering.Add(new SSGroupHeader($"<size=20>{menu.Name}</size>", true));

            if (RenderMenu(menu))
            {
                _shownMenus.Add(menu);
            }
            else // If the menu has no contents, remove the header
            {
                Rendering.RemoveAt(Rendering.Count - 1);
            }
        }
    }

    private void RenderDynamicMenus()
    {
        // Get the options for the dropdown
        _selectorOptions.Clear();
        _selectorOptions.Add("No menu");
        _selectorOptions.AddRange(SSSHandler.RegisteredMenus
            .Where(x => x.InternalHasPermission(TargetPlayer) && x.Type == MenuType.Dynamic).OrderBy(x => x.Name)
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
        Rendering.Add(new SSGroupHeader("<b>Menus</b>"));

        // Show dropdown selection
        MenuSelection = new SSDropdownSetting(-3, "Selected dynamic menu", _selectorOptions.ToArray(), index,
            hint: "Determines which menu is shown below. Select \'No menu\' to not show a menu.");
        Rendering.Add(MenuSelection);

        // Render the selected menu or empty message
        if (_selectedDynamicMenu == null || !_selectedDynamicMenu.InternalHasPermission(TargetPlayer))
        {
            Rendering.Add(new SSTextArea(-4, "Currently you have no valid menu selected!"));
        }
        else if (RenderMenu(_selectedDynamicMenu))
        {
            _shownMenus.Add(_selectedDynamicMenu);
        }
        else
        {
            Rendering.Add(new SSTextArea(-4, "The menu is currently empty."));
        }
    }

    private void RenderStaticMenus()
    {
        var hasMenu = false;

        Rendering.Add(new SSGroupHeader("<b>Settings</b>"));
        foreach (var menu in SSSHandler.RegisteredMenus
                     .Where(x => x.Type == MenuType.Static && x.InternalHasPermission(TargetPlayer))
                     .OrderByDescending(x => x.Priority))
        {
            Rendering.Add(new SSGroupHeader($"<size=20>{menu.Name}</size>", true));

            if (RenderMenu(menu))
            {
                hasMenu = true;
                _shownMenus.Add(menu);
            }
            else // If the menu has no contents, remove the header
            {
                Rendering.RemoveAt(Rendering.Count - 1);
            }
        }

        if (!hasMenu) // If there are no menus for this player, show a message
        {
            Rendering.Add(new SSTextArea(-2, UtilitiesPlugin.PluginConfig.NoSettingsText,
                textAlignment: TextAlignmentOptions.Top));
        }
    }

    private bool RenderMenu(MenuBase menu)
    {
        var addedItem = false;
        try
        {
            foreach (var item in menu.GetSettings(TargetPlayer))
            {
                item.RenderForMenu(menu, this);
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