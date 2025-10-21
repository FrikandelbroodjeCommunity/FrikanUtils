using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

/// <summary>
/// A factory for a setting that needs to be added to either the <see cref="GlobalClientSettingsMenu"/> or <see cref="GlobalServerSettingsMenu"/>.
/// </summary>
public interface IGlobalSetting
{
    /// <summary>
    /// The label shown for this setting.
    /// Will be used for sorting the settings.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Whether this setting is part of the <see cref="GlobalServerSettingsMenu"/> (<c>true</c>),
    /// or <see cref="GlobalClientSettingsMenu"/> (<c>false</c>).
    /// </summary>
    public bool ServerOnly { get; }

    /// <summary>
    /// Factory function that creates the actual setting. The setting <b>must</b> use the given settingId.
    /// </summary>
    /// <param name="settingId">ID the setting should have</param>
    /// <returns>Created setting</returns>
    public SettingsBase Get(ushort settingId);

    /// <summary>
    /// Determines whether the given player has permissions to view this global setting.
    /// </summary>
    /// <param name="player">Player to check permissions for</param>
    /// <returns>Whether the player has permissions</returns>
    public bool HasPermissions(Player player);
}