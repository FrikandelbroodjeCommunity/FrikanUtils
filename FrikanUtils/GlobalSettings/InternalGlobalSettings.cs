using FrikanUtils.ServerSpecificSettings;

namespace FrikanUtils.GlobalSettings;

internal static class InternalGlobalSettings
{
    private static readonly IGlobalSetting[] InternalSettings =
    [
        new HolidayOverrideSetting()
    ];

    private static readonly GlobalClientSettingsMenu ClientMenu = new();
    private static readonly GlobalServerSettingsMenu ServerMenu = new();

    internal static void RegisterInternalSettings()
    {
        foreach (var setting in InternalSettings)
        {
            GlobalSettingsHandler.RegisterSetting(setting);
        }

        SSSHandler.RegisterMenu(ClientMenu);
        SSSHandler.RegisterMenu(ServerMenu);
    }

    internal static void UnregisterInternalSettings()
    {
        SSSHandler.UnregisterMenu(ClientMenu);
        SSSHandler.UnregisterMenu(ServerMenu);

        foreach (var setting in InternalSettings)
        {
            GlobalSettingsHandler.UnregisterSetting(setting);
        }
    }
}