using System.Linq;
using LabApi.Features.Wrappers;
using MapGeneration;
using UnityEngine;

namespace FrikanUtils.Spawnpoints;

/// <summary>
/// Indicates a spawn point within a room. These are custom locations within a room.
/// </summary>
public class RoomSpawnPoint : ISpawnPoint
{
    /// <inheritdoc />
    public int Chance { get; set; }

    /// <summary>
    /// The room to spawn in.
    /// </summary>
    public RoomName Name;

    /// <summary>
    /// The offset to spawn in within the room.
    /// </summary>
    public Vector3 Position;

    /// <inheritdoc />
    public bool CanUse()
    {
        return Room.List.Any(x => x.Name == Name);
    }

    /// <inheritdoc />
    public Vector3 GetLocation()
    {
        var room = Room.Get(Name).FirstOrDefault();
        return room == null ? Vector3.zero : room.Transform.TransformPoint(Position);
    }
}