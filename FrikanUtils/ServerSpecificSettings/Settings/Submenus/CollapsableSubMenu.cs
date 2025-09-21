using System.Linq;
using FrikanUtils.ServerSpecificSettings.Helpers;
using FrikanUtils.ServerSpecificSettings.Menus;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ServerSpecificSettings.Settings.Submenus;

public abstract class CollapsableSubMenu : SubMenu
{
    public IServerSpecificSetting[] CachedSettings { get; private set; }

    protected abstract bool Label { get; }
    protected abstract bool DefaultCollapsed { get; }
    protected abstract bool FirstRenderCollapsed { get; }
    protected abstract ServerOnlyType CollapseSelectorServerType { get; }

    private MenuBase _menuOwner;
    private readonly ushort _settingId;

    public CollapsableSubMenu(ushort settingId)
    {
        _settingId = settingId;
    }

    public override void RenderForMenu(MenuBase menu, PlayerMenu playerMenu)
    {
        _menuOwner = menu;

        new TwoButtonSetting(_settingId, $"Collapse {Label}", "Expanded", "Collapsed", DefaultCollapsed,
                isServerOnly: CollapseSelectorServerType)
            .RegisterChangedAction(CollapsedUpdated)
            .RenderForMenu(menu, playerMenu);

        var shouldRender = !FirstRenderCollapsed;

        // Get the priorly rendered setting
        if (SSSHandler.TryGetField(playerMenu.TargetPlayer, menu, _settingId,
                out TwoButtonSetting renderedSetting))
        {
            shouldRender = renderedSetting.Value;
        }

        if (shouldRender)
        {
            CachedSettings = GetSettings(playerMenu.TargetPlayer).ToArray();

            foreach (var setting in CachedSettings)
            {
                setting.RenderForMenu(menu, playerMenu);
            }
        }
    }

    private void CollapsedUpdated(Player player, bool collapsed)
    {
        SSSHandler.UpdatePlayer(player, _menuOwner);
    }
}