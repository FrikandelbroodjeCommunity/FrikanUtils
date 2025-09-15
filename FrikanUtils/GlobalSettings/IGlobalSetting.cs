using FrikanUtils.ServerSpecificSettings.Settings;
using LabApi.Features.Wrappers;

namespace FrikanUtils.GlobalSettings;

public interface IGlobalSetting
{
    public bool ServerOnly { get; }

    public SettingsBase Get();
    public bool HasPermissions(Player player);
}