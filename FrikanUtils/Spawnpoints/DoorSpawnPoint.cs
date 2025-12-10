using System.Linq;
using LabApi.Features.Enums;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace FrikanUtils.Spawnpoints;

/// <summary>
/// Indicates a spawn point at a door. Will move the position slightly outside of the door.
/// </summary>
public class DoorSpawnPoint : ISpawnPoint
{
    /// <inheritdoc />
    public int Chance { get; set; }

    /// <summary>
    /// Name of the door to spawn at.
    /// </summary>
    public DoorName Name;

    /// <inheritdoc />
    public bool CanUse()
    {
        return Door.List.Any(x => x.DoorName == Name);
    }

    /// <inheritdoc />
    public Vector3 GetLocation()
    {
        var door = Door.List.FirstOrDefault(x => x.DoorName == Name);
        if (door == null)
        {
            return Vector3.zero;
        }

        return door.Transform.position + Vector3.up * 1.5f + door.Transform.forward * 3f;
    }
}