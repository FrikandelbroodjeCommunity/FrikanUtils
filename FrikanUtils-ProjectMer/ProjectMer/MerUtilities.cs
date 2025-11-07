using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdminToys;
using FrikanUtils.FileSystem;
using FrikanUtils.ProjectMer.Patches;
using FrikanUtils.Utilities;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using LiteNetLib4Mirror.Open.Nat;
using MEC;
using Mirror;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable.Schematics;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;
using Object = UnityEngine.Object;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace FrikanUtils.ProjectMer;

/// <summary>
/// Utilities related to MER schematics.
/// </summary>
public static partial class MerUtilities
{
    internal static readonly List<Interactable> RegisteredPickups = [];

    /// <summary>
    /// Find objects with the given name in a schematic object.
    /// </summary>
    /// <param name="obj">Schematic object to search through</param>
    /// <param name="name">Name of the object(s) to search</param>
    /// <returns>Enumerable for the transform(s) of the found object(s)</returns>
    public static IEnumerable<Transform> FindNamedObjects(this SchematicObject obj, string name)
    {
        return obj.AttachedBlocks
            .Where(schematicObj => schematicObj.name == name)
            .Select(x => x.transform);
    }

    /// <summary>
    /// Make this primitive toy interactable.
    /// </summary>
    /// <param name="transform">Primitive toy</param>
    /// <param name="pickingUp">Action executed when a player starts the pickup</param>
    /// <param name="id">ID to use for distinguishing between interactables</param>
    /// <param name="duration">How long the player needs to interact to complete</param>
    public static void RegisterPickupAction(this Transform transform, Func<Player, string, bool> pickingUp, string id,
        float duration)
        => transform.RegisterPickupAction(pickingUp, null, id, duration);

    /// <summary>
    /// Make this primitive toy interactable.
    /// </summary>
    /// <param name="transform">Primitive toy</param>
    /// <param name="action">Action exectued when a player finishes the pickup</param>
    /// <param name="id">ID to use for distinguishing between interactables</param>
    /// <param name="duration">How long the player needs to interact to complete</param>
    public static void RegisterPickupAction(this Transform transform, Action<Player, string> action, string id,
        float duration)
        => transform.RegisterPickupAction(null, action, id, duration);

    /// <summary>
    /// Make this primitive toy interactable.
    /// </summary>
    /// <param name="transform">Primitive toy</param>
    /// <param name="pickingUp">Action executed when a player starts the pickup</param>
    /// <param name="action">Action exectued when a player finishes the pickup</param>
    /// <param name="id">ID to use for distinguishing between interactables</param>
    /// <param name="duration">How long the player needs to interact to complete</param>
    public static void RegisterPickupAction(this Transform transform, Func<Player, string, bool> pickingUp,
        Action<Player, string> action, string id, float duration)
    {
        if (!transform.TryGetComponent(out PrimitiveObjectToy toy))
        {
            Logger.Warn("Failed to spawn. No initial toy!");
            return;
        }

        var created = InteractableToy.Create(toy.transform).Base;
        created.NetworkInteractionDuration = duration;

        switch (toy.PrimitiveType)
        {
            case PrimitiveType.Sphere:
                created.NetworkShape = InvisibleInteractableToy.ColliderShape.Sphere;
                break;
            case PrimitiveType.Capsule:
            case PrimitiveType.Cylinder:
                created.NetworkShape = InvisibleInteractableToy.ColliderShape.Capsule;
                break;
            default:
                created.NetworkShape = InvisibleInteractableToy.ColliderShape.Box;
                break;
        }

        // Make the original toy uncollidable so the new one is the only collidable
        toy.NetworkPrimitiveFlags &= ~PrimitiveFlags.Collidable;

        RegisteredPickups.Add(new Interactable
        {
            Toy = created,
            Id = id,
            PickingUp = pickingUp,
            PickedUp = action
        });
    }

    /// <summary>
    /// Spawn doors for every "SpawnDoor" object in the schematic.
    /// Keep in mind, you <b>must</b> loop over the returned enumerable to ensure it works.
    /// </summary>
    /// <param name="obj">Schematic to spawn doors for</param>
    /// <returns>Spawned door information</returns>
    public static IEnumerable<DoorInfo> SpawnDoors(this SchematicObject obj)
    {
        var door = NetworkClient.prefabs.Values.First(x => x.name.Contains("EZ"));

        foreach (var schematicObj in obj.AttachedBlocks.Where(x => x.name.ContainsIgnoreCase("SpawnDoor")))
        {
            var transform = schematicObj.transform;
            var doorObj = Object.Instantiate(door, transform.position, transform.rotation);
            NetworkServer.Spawn(doorObj);

            yield return new DoorInfo
            {
                Door = doorObj.GetComponent<DoorVariant>(),
                DoorId = schematicObj.name
            };
        }
    }

    /// <summary>
    /// Divide the players into teams and spawn them around the map (on their team spawnpoints)
    /// </summary>
    /// <param name="map"></param>
    /// <param name="teamCount"></param>
    public static void SpawnTeams(this SchematicObject map, int teamCount)
    {
        if (teamCount > TeamUtilities.TeamRoles.Length)
        {
            Logger.Error($"Cannot assign more teams than are available {teamCount} > {TeamUtilities.TeamRoles.Length}");
            return;
        }

        var positions = new Vector3[teamCount];
        for (var i = 0; i < teamCount; i++)
        {
            var role = TeamUtilities.TeamRoles[i];
            var found = false;

            foreach (var obj in map.FindNamedObjects($"SpawnTeam{role}"))
            {
                positions[i] = obj.position;
                found = true;
                break;
            }

            if (!found)
            {
                Logger.Error($"Could not find named object: SpawnTeam{role}");
                return;
            }
        }

        TeamUtilities.AssignEqualTeams(positions);
    }
}