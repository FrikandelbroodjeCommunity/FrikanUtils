using Interactables.Interobjects.DoorUtils;

namespace FrikanUtils.ProjectMer;

/// <summary>
/// Represents the information for a spawned door
/// </summary>
public struct DoorInfo
{
    /// <summary>
    /// The door that was spawned.
    /// </summary>
    public DoorVariant Door;

    /// <summary>
    /// The full name of the object that caused the door to spawn.
    /// </summary>
    public string DoorId;
}