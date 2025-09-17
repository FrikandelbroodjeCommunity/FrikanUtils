using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
public static class MerUtilities
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
    /// Try to find the schematic and spawn it.<br/>
    /// Will use the current holiday as a prefix filter. For exact formatting view the README.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="action"></param>
    /// <param name="folder"></param>
    /// <param name="ignoreHoliday"></param>
    public static async void FindAndSpawnSchematic(string file, Vector3 position,
        Quaternion rotation, Action<SchematicObject> action = null, string folder = "Maps", bool ignoreHoliday = false)
    {
        var data = await FileHandler.SearchFile<SchematicObjectDataList>(file, folder, true);
        if (data == null)
        {
            Logger.Warn($"Could not find {file} in folder {folder}!");
            return;
        }

        data.Path = Path.Combine("RetrievedUsingFileHandler", folder, file);

        // Because this is an async context, go back to the main thread using a coroutine to spawn the schematic
        SchematicObject spawned;
        Timing.CallDelayed(Timing.WaitForOneFrame,
            () =>
            {
                spawned = data.SpawnSchematic(position, rotation, ignoreHoliday);
                action?.Invoke(spawned);
            }
        );
    }

    /// <summary>
    /// Spawn a schematic based on this object data list.<br/>
    /// Will use the current holiday as a prefix filter. For exact formatting view the README.
    /// </summary>
    /// <param name="data">Schematic data</param>
    /// <param name="position">Position to spawn the schematic at</param>
    /// <param name="rotation">Rotation to apply to the schematic</param>
    /// <param name="ignoreHoliday">When enabled it will ignore the holiday specific elements (forced to use None)</param>
    /// <returns>Spawned schematic</returns>
    public static SchematicObject SpawnSchematic(this SchematicObjectDataList data, Vector3 position,
        Quaternion rotation, bool ignoreHoliday = false)
    {
        var rootToy = Object.Instantiate(PrefabManager.PrimitiveObject);
        rootToy.NetworkPrimitiveFlags = PrimitiveFlags.None;
        rootToy.transform.position = position;
        rootToy.transform.rotation = rotation;

        NetworkServer.Spawn(rootToy.gameObject);

        var schematic = rootToy.gameObject.AddComponent<SchematicObject>();
        if (!ignoreHoliday)
        {
            HolidayMerPatch.ApplicableSchematics.Add(schematic);
        }

        return schematic.Init(data);
    }

    public static void RegisterPickupAction(this Transform transform, Func<Player, string, bool> pickingUp, string id,
        float duration)
        => transform.RegisterPickupAction(pickingUp, null, id, duration);

    public static void RegisterPickupAction(this Transform transform, Action<Player, string> action, string id,
        float duration)
        => transform.RegisterPickupAction(null, action, id, duration);

    public static void RegisterPickupAction(this Transform transform, Func<Player, string, bool> pickingUp,
        Action<Player, string> action, string id, float duration)
    {
        if (!transform.TryGetComponent(out PrimitiveObjectToy toy))
        {
            Logger.Info("Failed to spawn. No initial toy!");
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