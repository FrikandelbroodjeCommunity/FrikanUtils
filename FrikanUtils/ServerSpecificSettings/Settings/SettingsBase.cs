using FrikanUtils.ServerSpecificSettings.Helpers;
using FrikanUtils.ServerSpecificSettings.Menus;
using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

public abstract class SettingsBase : IServerSpecificSetting
{
    public ushort? SettingId { get; }
    public string MenuOwner { get; private set; }
    public readonly ServerOnlyType ServerOnlyType;

    public abstract ServerSpecificSettingBase Base { get; }

    public string Label
    {
        get => Base.Label;
        set => Base.SendLabelUpdate(value);
    }

    public string HintDescription
    {
        get => Base.HintDescription;
        set => Base.SendHintUpdate(value);
    }

    internal Player Player;

    internal int Id
    {
        get => Base.SettingId;
        set => Base.SettingId = value;
    }

    protected SettingsBase(ushort? settingId, ServerOnlyType serverOnlyType)
    {
        SettingId = settingId;
        ServerOnlyType = serverOnlyType;
    }

    public void UpdateBase(string newLabel, string newHintDescription, bool applyOverride = true)
    {
        Base.SendUpdate(newLabel, newHintDescription, applyOverride);
    }

    public void RenderForMenu(MenuBase menu, PlayerMenu playerMenu)
    {
        Player = playerMenu.TargetPlayer;
        Id = playerMenu.IDHandler.GetId(menu.Name, SettingId, ServerOnlyType);
        MenuOwner = menu.Name;
        playerMenu.RenderingItems.Add(this);
        playerMenu.Rendering.Add(Base);
    }
}