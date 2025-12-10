using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace FrikanUtils.Spawnpoints;

/// <summary>
/// Represents a spawn location that can be used to gather one, or more, positions to spawn things at.
/// TODO: Optionally, prevent duplicate spawn locations
/// </summary>
public class SpawnLocation
{
    private static readonly Random Random = new();

    /// <summary>
    /// The minimum amount of spawn locations to generate.
    /// </summary>
    public int Min = 1;

    /// <summary>
    /// The maximum amount of spawn locations to generate.
    /// </summary>
    public int Max = 1;

    /// <summary>
    /// The instructions to use when generating spawn positions.
    /// </summary>
    public ISpawnPoint[] Points;

    /// <summary>
    /// Generate all spawn positions.
    /// </summary>
    /// <returns>Enumerable of positions</returns>
    public IEnumerable<Vector3> GetLocations()
    {
        var max = Points.Sum(point => point.Chance);
        for (var i = 0; i < Random.Next(Min, Max + 1); i++)
        {
            var random = Random.Next(0, max);
            foreach (var point in Points)
            {
                random -= point.Chance;
                if (random >= 0) continue;

                yield return point.GetLocation();
                break;
            }
        }
    }
}