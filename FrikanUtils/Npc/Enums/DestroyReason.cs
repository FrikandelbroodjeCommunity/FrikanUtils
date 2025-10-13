namespace FrikanUtils.Npc.Enums;

/// <summary>
/// The reason an NPC was destroyed.
/// </summary>
public enum DestroyReason
{
    /// <summary>
    /// The dummy used for the NPC was killed.
    /// </summary>
    Died,

    /// <summary>
    /// The NPC was removed because it reached its target.
    /// </summary>
    ReachedTarget,

    /// <summary>
    /// The NPC was removed because it got too far away from a location/target.
    /// </summary>
    OutsideOfRange,

    /// <summary>
    /// The NPC was removed by a plugin.
    /// </summary>
    Removal,

    /// <summary>
    /// The NPC player or component was destroyed.
    /// </summary>
    Cleanup
}