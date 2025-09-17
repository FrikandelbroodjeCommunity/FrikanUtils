using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

public interface IGlobalSetting
{
    public string Label { get; }
    public bool ServerOnly { get; }

    public SettingsBase Get(ushort settingId);
    public bool HasPermissions(Player player);
}