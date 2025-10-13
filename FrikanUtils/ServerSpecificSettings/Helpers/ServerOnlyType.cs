namespace FrikanUtils.ServerSpecificSettings.Helpers;

/// <summary>
/// The different ways of storing the values for Server Specific Settings.
/// </summary>
public enum ServerOnlyType
{
    /// <summary>
    /// It is not server only, instead the value is stored on the client.
    /// </summary>
    Client,

    /// <summary>
    /// It is server only, each player gets their own value.
    /// </summary>
    ServerOnly,

    /// <summary>
    /// It is server only, whenever a value is received from 1 player, all players get updated with this new value.
    /// </summary>
    GlobalServerOnly
}