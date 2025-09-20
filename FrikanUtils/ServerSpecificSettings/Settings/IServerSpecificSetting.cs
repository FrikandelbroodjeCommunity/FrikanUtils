using FrikanUtils.ServerSpecificSettings.Menus;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public interface IServerSpecificSetting
{
    internal void RenderForMenu(MenuBase menu, PlayerMenu playerMenu);
}