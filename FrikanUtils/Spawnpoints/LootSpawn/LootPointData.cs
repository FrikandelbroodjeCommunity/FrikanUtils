using MapGeneration;
using UnityEngine;

namespace FrikanUtils.Spawnpoints.LootSpawn;

/// <summary>
/// Data retrieved to indicate the room and position an item should be spawned at.
/// </summary>
public class LootPointData
{
    /// <summary>
    /// Name of the room the <see cref="Position"/> is an offset in.
    /// </summary>
    public RoomName RoomName;

    /// <summary>
    /// The offset to the root of the room.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// Create a new instance of the data.
    /// </summary>
    /// <param name="roomName">Name of the room</param>
    /// <param name="position">Position within the room</param>
    public LootPointData(RoomName roomName, Vector3 position)
    {
        RoomName = roomName;
        Position = position;
    }
}