using FrikanUtils.ServerSpecificSettings.Settings;

namespace FrikanUtils.GlobalSettings;

public class GlobalSetting
{
    public SettingsBase Setting { get; }
    public string[] RequiredPermissions { get; }

    public bool IsServerOnly => Setting.Base.IsServerOnly;

    public GlobalSetting(SettingsBase setting)
    {
        Setting = setting;
        RequiredPermissions = null;
    }

    public GlobalSetting(SettingsBase setting, params string[] permissions)
    {
        Setting = setting;
        RequiredPermissions = permissions;
    }

    public SettingsBase GetClone() => Setting.Clone();
}