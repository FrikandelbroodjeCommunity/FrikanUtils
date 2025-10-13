using FrikanUtils.ServerSpecificSettings.Helpers;
using FrikanUtils.ServerSpecificSettings.Menus;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Settings.Submenus;

/// <summary>
/// A menu which shows a group of settings only when the player has the menu expanded.
/// </summary>
public abstract class CollapsableSubMenu : SubMenu
{
    /// <summary>
    /// Label to show on the collapse setting.
    /// </summary>
    protected abstract string Label { get; }

    /// <summary>
    /// Whether the menu should be collapsed by default.
    /// </summary>
    protected abstract bool DefaultCollapsed { get; }

    /// <summary>
    /// Whether to always render the full menu at first.
    /// If this is enabled the settings can be gotten, even if the menu is hidden.
    /// Otherwise, settings may not be loaded when you try to get them.
    /// </summary>
    protected abstract bool FirstRenderCollapsed { get; }

    /// <summary>
    /// The <see cref="ServerOnlyType"/> for the collapse setting.
    /// </summary>

    protected abstract ServerOnlyType CollapseSelectorServerType { get; }

    private readonly ushort _settingId;

    /// <summary>
    /// The new collapsable sub menu, the ID the collapse setting should be given.
    /// </summary>
    /// <param name="settingId">Free ID for the collapse setting</param>
    public CollapsableSubMenu(ushort settingId)
    {
        _settingId = settingId;
    }

    /// <inheritdoc />
    protected override void RenderContents(MenuBase menu, PlayerMenu playerMenu)
    {
        new TwoButtonSetting(_settingId, $"Collapse: {Label}", "Expanded", "Collapsed", DefaultCollapsed,
                isServerOnly: CollapseSelectorServerType)
            .RegisterChangedAction(CollapsedUpdated)
            .RenderForMenu(menu, playerMenu);

        var shouldRender = !FirstRenderCollapsed;

        // Get the priorly rendered setting
        if (SSSHandler.TryGetField(playerMenu.TargetPlayer, menu, _settingId, out TwoButtonSetting renderedSetting))
        {
            shouldRender = !renderedSetting.Value;
        }

        if (shouldRender)
        {
            base.RenderContents(menu, playerMenu);
        }
    }

    private void CollapsedUpdated(Player player, bool oldValue, bool collapsed)
    {
        if (oldValue != collapsed)
        {
            SSSHandler.UpdatePlayer(player, OwnerMenu, false);
        }
    }
}