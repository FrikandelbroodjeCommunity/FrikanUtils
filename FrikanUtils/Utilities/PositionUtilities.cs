using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrikanUtils.Utilities;

/// <summary>
/// Utilities related to positioning things in the world.
/// </summary>
public static class PositionUtilities
{
    /// <summary>
    /// Get all positions in order to create a circle around <c>vector3.zero</c>.
    /// The radius gets determined by the amount of positions on the circle, attempting to space them out at a similar distance for each count.
    /// </summary>
    /// <param name="count">The amount of positions on the circle</param>
    /// <param name="radiusMultiplier">The multiplier applied to the count to get the radius</param>
    /// <param name="minRadius">The minimum radius of the circle</param>
    /// <param name="maxRadius">The maximum radius of the circle</param>
    /// <returns>Enumerable for each of the positions</returns>
    public static IEnumerable<PositionAndRotation> GetCirclePositions(int count, float radiusMultiplier = 0.5f,
        float minRadius = 0.0f, float maxRadius = float.MaxValue)
    {
        var radius = Math.Min(Math.Max(radiusMultiplier * count, minRadius), maxRadius);
        var angle = 360f / count;

        for (var i = 0; i < count; i++)
        {
            var currentAngle = angle * i;
            var currentRad = currentAngle * Mathf.Deg2Rad;
            var offset = new Vector3(radius * Mathf.Cos(currentRad), 0, radius * Mathf.Sin(currentRad));

            yield return new PositionAndRotation(offset, Vector3.up * -currentAngle);
        }
    }

    /// <summary>
    /// Representation of a position and rotation.
    /// </summary>
    public struct PositionAndRotation
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public PositionAndRotation(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public PositionAndRotation(Vector3 position, Vector3 rotation) : this(position, Quaternion.Euler(rotation))
        {
        }
    }
}