using UnityEngine;

namespace FrikanUtils.Spawnpoints;

/// <summary>
/// Represents a point at which an item, object, or player can be spawned.
/// </summary>
public interface ISpawnPoint
{
    /// <summary>
    /// The chance this point will be used out of a list.
    /// </summary>
    public int Chance { get; set; }

    /// <summary>
    /// Whether the spawn point can be used. (e.g. should return false if the room it attempts to target was not spawned in the current round).
    /// </summary>
    /// <returns>Whether the point can be used</returns>
    public bool CanUse();

    /// <summary>
    /// Get the absolute position the item, object, or player needs to be spawned at.
    /// </summary>
    /// <returns>Absolute spawn position</returns>
    public Vector3 GetLocation();
}