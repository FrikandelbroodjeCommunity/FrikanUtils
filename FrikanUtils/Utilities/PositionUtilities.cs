using System.Collections.Generic;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace FrikanUtils.Utilities;

/// <summary>
/// Utilities related to positioning things in the world.
/// </summary>
public static class PositionUtilities
{
    /// <summary>
    /// Get all offsets in order to create a cricle.
    /// Will approximate the radius by using the distance between points on the circle.
    /// </summary>
    /// <param name="count">The amount of positions on the circle</param>
    /// <param name="distance">Distance between points on the circle</param>
    /// <returns>Positions and rotations of objects on the circle</returns>
    public static IEnumerable<PositionAndRotation> GetAutoCirclePositions(int count, float distance)
        => GetCirclePositions(count, count * distance / (2 * Mathf.PI));


    /// <summary>
    /// Get all offsets in order to create a circle.
    /// </summary>
    /// <param name="count">Amount of points on the circle</param>
    /// <param name="radius">Radius of the circle</param>
    /// <returns>Positions and rotations of objects on the circle</returns>
    public static IEnumerable<PositionAndRotation> GetCirclePositions(int count, float radius)
    {
        Logger.Debug($"Generating circle positions for radius: {radius}", UtilitiesPlugin.PluginConfig.Debug);
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