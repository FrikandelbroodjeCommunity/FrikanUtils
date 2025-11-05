namespace FrikanUtils.Npc.Enums;

/// <summary>
/// The movement state of the NPC.
/// </summary>
public enum NpcState
{
    /// <summary>
    /// The NPC is paused, no longer moving.
    /// </summary>
    Paused,

    /// <summary>
    /// The NPC is walking towards a goal.
    /// </summary>
    Walking,

    /// <summary>
    /// The NPC is sprinting towards a goal.
    /// </summary>
    Sprinting
}