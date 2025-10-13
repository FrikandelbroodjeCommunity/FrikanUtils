namespace FrikanUtils.Npc.Enums;

/// <summary>
/// The action the NPC should take when it reaches its target.
/// </summary>
public enum ReachTargetAction
{
    /// <summary>
    /// The NPC should halt after reaching its target.
    /// </summary>
    Pause,

    /// <summary>
    /// The NPC should be destroyed when reaching its target.
    /// </summary>
    Destroy,

    /// <summary>
    /// A custom action defined for the NPC is used.
    /// </summary>
    CustomAction
}