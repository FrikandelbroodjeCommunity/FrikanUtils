using System.Linq;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace FrikanUtils.Spawnpoints.LootSpawn;

/// <summary>
/// Indicates a spawn point at a loot location. These are preset locations within a room.
/// </summary>
public class LootSpawnPoint : ISpawnPoint
{
    /// <inheritdoc />
    public int Chance { get; set; }

    /// <summary>
    /// The <see cref="LootPoint"/> to spawn at.
    /// </summary>
    public LootPoint Point;

    private LootPointData _data;

    /// <inheritdoc />
    public bool CanUse()
    {
        _data ??= Point.GetPointData();
        return _data != null && Room.List.Any(x => x.Name == _data.RoomName);
    }

    /// <inheritdoc />
    public Vector3 GetLocation()
    {
        _data ??= Point.GetPointData();

        var room = Room.List.FirstOrDefault(x => x.Name == _data.RoomName);
        return room == null ? Vector3.zero : room.Transform.TransformPoint(_data.Position);
    }
}