using System.Linq;
using FrikanUtils.ServerSpecificSettings.Helpers;
using FrikanUtils.ServerSpecificSettings.Menus;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Settings.Submenus;

public abstract class CollapsableSubMenu : SubMenu
{
    protected abstract string Label { get; }
    protected abstract bool DefaultCollapsed { get; }
    protected abstract bool FirstRenderCollapsed { get; }
    protected abstract ServerOnlyType CollapseSelectorServerType { get; }

    private readonly ushort _settingId;

    public CollapsableSubMenu(ushort settingId)
    {
        _settingId = settingId;
    }

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