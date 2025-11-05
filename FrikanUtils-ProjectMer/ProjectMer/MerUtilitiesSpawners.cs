using System;
using AdminToys;
using FrikanUtils.ProjectMer.Patches;
using FrikanUtils.Utilities;
using Mirror;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable.Schematics;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;
using Object = UnityEngine.Object;

namespace FrikanUtils.ProjectMer;

public static partial class MerUtilities
{
    /// <summary>
    /// Try to find the schematic and spawn it.<br/>
    /// Will use the current holiday as a prefix filter. For exact formatting view the README.
    /// </summary>
    /// <param name="file">Name of the file to find</param>
    /// <param name="position">Position to spawn the schematic at</param>
    /// <param name="rotation">Rotation to spawn the schamtic with</param>
    /// <param name="action">Action executed on the main thread after spawning</param>
    /// <param name="folder">Folder to search for the file in</param>
    /// <param name="ignoreHoliday">Whether to ignore the holiday filter</param>
    public static async void FindAndSpawnSchematic(string file, Vector3 position,
        Quaternion rotation, Action<SchematicObject> action = null, string folder = "Maps", bool ignoreHoliday = false)
    {
        var data = await LoadSchematicData(file, folder);
        if (data == null)
        {
            Logger.Warn($"Could not find {file} in folder {folder}!");
            return;
        }

        // Because this is an async context, go back to the main thread using a coroutine to spawn the schematic
        AsyncUtilities.ExecuteOnMainThread(() =>
        {
            var spawned = data.SpawnSchematic(position, rotation, ignoreHoliday);
            action?.Invoke(spawned);
        });
    }
    
    /// <summary>
    /// Try to find the schematic, allowing additional data, and spawn it.<br/>
    /// Will use the current holiday as a prefix filter. For exact formatting view the README.
    /// </summary>
    /// <param name="schematic">The name of the schematic (without the <c>.zip</c> extension)</param>
    /// <param name="position">Position to spawn the schematic at</param>
    /// <param name="rotation">Rotation to spawn the schamtic with</param>
    /// <param name="action">Action executed on the main thread after spawning</param>
    /// <param name="folder">Folder to search for the file in</param>
    /// <param name="ignoreHoliday">Whether to ignore the holiday filter</param>
    public static async void FindAndSpawnFullSchematic(string schematic, Vector3 position,
        Quaternion rotation, Action<SchematicObject> action = null, string folder = "Maps", bool ignoreHoliday = false)
    {
        var data = await LoadFullSchematic(schematic, folder);
        if (data == null)
        {
            Logger.Warn($"Could not find {schematic}.zip in folder {folder}!");
            return;
        }

        // Because this is an async context, go back to the main thread using a coroutine to spawn the schematic
        AsyncUtilities.ExecuteOnMainThread(() =>
        {
            var spawned = data.SpawnSchematic(position, rotation, ignoreHoliday);
            action?.Invoke(spawned);
        });
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
    
}