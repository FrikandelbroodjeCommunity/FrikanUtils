using FrikanUtils.ServerSpecificSettings.Helpers;
using FrikanUtils.ServerSpecificSettings.Menus;
using LabApi.Features.Wrappers;
using UserSettings.ServerSpecific;

namespace FrikanUtils.ServerSpecificSettings.Settings;

/// <summary>
/// An individual setting that can be shown in the Server Specific Setting system.
/// </summary>
public abstract class SettingsBase : IServerSpecificSetting
{
    /// <summary>
    /// The optional ID of the setting.
    /// If no ID is given, one will automatically be assigned, but it is not possible to retrieve the instance later.
    /// </summary>
    public ushort? SettingId { get; }

    /// <summary>
    /// The <see cref="MenuBase.Name"/> of the menu that owns this setting.
    /// </summary>
    public string MenuOwner { get; private set; }

    /// <summary>
    /// How to store the value for this setting.
    /// </summary>
    public readonly ServerOnlyType ServerOnlyType;

    /// <summary>
    /// The instance used by the base game.
    /// </summary>
    public abstract ServerSpecificSettingBase Base { get; }

    /// <summary>
    /// Label displayed for this setting.
    /// </summary>
    public string Label
    {
        get => Base.Label;
        set => Base.SendLabelUpdate(value, true, UpdateFilter);
    }

    /// <summary>
    /// Additional explanation for the setting.
    /// </summary>
    public string HintDescription
    {
        get => Base.HintDescription;
        set => Base.SendHintUpdate(value, true, UpdateFilter);
    }

    internal Player Player;

    internal int Id
    {
        get => Base.SettingId;
        set => Base.SettingId = value;
    }

    /// <summary>
    /// Create the setting with the given id and type.
    /// </summary>
    /// <param name="settingId">The optional ID of the setting</param>
    /// <param name="serverOnlyType">How to store the setting</param>
    protected SettingsBase(ushort? settingId, ServerOnlyType serverOnlyType)
    {
        SettingId = settingId;
        ServerOnlyType = serverOnlyType;
    }

    /// <summary>
    /// Copy the value of another setting, into this setting.
    /// Only used for settings which actually have values determined by the user.
    /// </summary>
    /// <param name="setting">The setting to copy the value from</param>
    public virtual void CopyValue(SettingsBase setting)
    {
    }

    /// <summary>
    /// Update the label and description for this setting at the same time.
    /// </summary>
    /// <param name="newLabel">The new label to display for the setting</param>
    /// <param name="newHintDescription">The new hint to display for the setting</param>
    /// <param name="applyOverride">Whether to apply the change immediately</param>
    public void UpdateBase(string newLabel, string newHintDescription, bool applyOverride = true)
    {
        Base.SendUpdate(newLabel, newHintDescription, applyOverride, UpdateFilter);
    }

    /// <inheritdoc />
    public void RenderForMenu(MenuBase menu, PlayerMenu playerMenu)
    {
        Player = playerMenu.TargetPlayer;
        Id = playerMenu.IDHandler.GetId(menu.Name, SettingId);
        MenuOwner = menu.Name;
        playerMenu.RenderingItems.Add(this);
        playerMenu.Rendering.Add(Base);
    }

    /// <summary>
    /// Filter that needs to be used when updating fields.
    /// Will make sure only the player the field belongs to gets updated.
    /// </summary>
    protected bool UpdateFilter(ReferenceHub player)
    {
        return Player.ReferenceHub == player;
    }
}