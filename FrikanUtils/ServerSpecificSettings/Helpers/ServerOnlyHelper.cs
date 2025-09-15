namespace FrikanUtils.ServerSpecificSettings.Helpers;

public static class ServerOnlyHelper
{
    public static bool IsServerOnly(this ServerOnlyType type)
    {
        return type is ServerOnlyType.ServerOnly or ServerOnlyType.GlobalServerOnly;
    }

    public static bool IsGlobalSetting(this ServerOnlyType type)
    {
        return type == ServerOnlyType.GlobalServerOnly;
    }
}