namespace FrikanUtils.Npc.Enums;

/// <summary>
/// The action the NPC should take when it is too far from a location/target.
/// </summary>
public enum OutOfRangeAction
{
    /// <summary>
    /// The NPC will halt until it is within range again.
    /// </summary>
    Pause,

    /// <summary>
    /// The NPC will clear the location/target and halt until a new target within range is defined.
    /// </summary>
    StopFollowing,

    /// <summary>
    /// The NPC will be destroyed.
    /// </summary>
    Destroy,

    /// <summary>
    /// The NPC will teleport to the location/target so it is no longer out of range.
    /// </summary>
    Teleport,

    /// <summary>
    /// A custom action defined for the NPC is used.
    /// </summary>
    CustomAction
}