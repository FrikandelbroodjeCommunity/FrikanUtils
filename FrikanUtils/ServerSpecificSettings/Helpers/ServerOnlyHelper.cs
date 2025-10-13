namespace FrikanUtils.ServerSpecificSettings.Helpers;

/// <summary>
/// Small helper utility to determine how to interact with the <see cref="ServerOnlyType"/>.
/// </summary>
public static class ServerOnlyHelper
{
    /// <summary>
    /// Determines whether this <see cref="ServerOnlyType"/> is one of the server only options.
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <returns>Whether the type is server only</returns>
    public static bool IsServerOnly(this ServerOnlyType type)
    {
        return type is ServerOnlyType.ServerOnly or ServerOnlyType.GlobalServerOnly;
    }
}