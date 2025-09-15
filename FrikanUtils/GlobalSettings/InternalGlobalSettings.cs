using FrikanUtils.ServerSpecificSettings;

namespace FrikanUtils.GlobalSettings;

internal static class InternalGlobalSettings
{
    private static readonly IGlobalSetting[] InternalSettings =
    [
        new HolidayOverrideSetting()
    ];

    private static readonly GlobalClientSettingsMenu _clientMenu = new();
    private static readonly GlobalServerSettingsMenu _serverMenu = new();

    internal static void RegisterInternalSettings()
    {
        foreach (var setting in InternalSettings)
        {
            GlobalSettingsHandler.RegisterSetting(setting);
        }

        SSSHandler.RegisterMenu(_clientMenu);
        SSSHandler.RegisterMenu(_serverMenu);
    }

    internal static void UnregisterInternalSettings()
    {
        SSSHandler.UnregisterMenu(_clientMenu);
        SSSHandler.UnregisterMenu(_serverMenu);

        foreach (var setting in InternalSettings)
        {
            GlobalSettingsHandler.UnregisterSetting(setting);
        }
    }
}