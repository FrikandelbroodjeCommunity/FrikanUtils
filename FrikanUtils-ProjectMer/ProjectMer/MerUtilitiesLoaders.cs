using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrikanUtils.FileSystem;
using FrikanUtils.Utilities;
using LabApi.Features.Console;
using ProjectMER.Features.Serializable.Schematics;
using Utf8Json;

namespace FrikanUtils.ProjectMer;

public static partial class MerUtilities
{
    /// <summary>
    /// Try to find the schematic. If no schematic was found will return null.
    ///
    /// The given action is only triggered when a map is found.
    /// </summary>
    /// <param name="file">Name of the file to find</param>
    /// <param name="action">Action executed on the main thread after spawning</param>
    /// <param name="folder">Folder to search for the file in</param>
    /// <returns>The schematic data</returns>
    public static async Task<SchematicObjectDataList> LoadSchematicData(string file, string folder = "Maps",
        Action<SchematicObjectDataList> action = null)
    {
        var data = await FileHandler.SearchFile<SchematicObjectDataList>(file, folder, true);
        if (data == null)
        {
            return null;
        }

        data.Path = Path.Combine("RetrievedUsingFileHandler", folder, file);

        if (action != null)
        {
            AsyncUtilities.ExecuteOnMainThread(() => { action.Invoke(data); });
        }

        return data;
    }

    /// <summary>
    /// Try to find the schematic zip, allowing additional data like animations. The zip file given will be unzipped. If no schematic was found will return null.
    ///
    /// The given action is only triggered when a map is found.
    /// </summary>
    /// <param name="schematic">The name of the schematic (without the <c>.zip</c> extension)</param>
    /// <param name="folder">Folder to search for the file in</param>
    /// <param name="action">Action executed on the main thread after spawning</param>
    /// <returns>The schematic data</returns>
    public static async Task<SchematicObjectDataList> LoadFullSchematic(string schematic, string folder = "Maps",
        Action<SchematicObjectDataList> action = null)
    {
        var zipPath = await FileHandler.SearchFullPath($"{schematic}.zip", folder);
        if (zipPath == null)
        {
            return null;
        }

        var dir = Path.GetDirectoryName(zipPath);
        if (dir == null)
        {
            Logger.Warn($"Failed to find directory after finding file for {schematic}");
            return null;
        }

        // Get the target folder and a temporary folder
        var targetFolder = Path.Combine(dir, schematic);
        var tempFolder = Path.Combine(dir, $"temp_{schematic}");

        var i = -1; // Make sure the tempfolder did not exist yet
        while (Directory.Exists(i < 0 ? tempFolder : $"{tempFolder}{i}"))
        {
            i++;
        }

        if (i >= 0)
        {
            tempFolder = $"{tempFolder}{i}";
        }

        var success = false;
        try
        {
            ZipFile.ExtractToDirectory(zipPath, tempFolder);
            success = true;
        }
        catch (Exception e)
        {
            Logger.Error($"An error occured while unpacking the {schematic} schematic:\n{e}");
        }

        if (!success)
        {
            Directory.Delete(tempFolder);
            return null;
        }

        // Copy everything from the temp folder to the actual target folder
        CopyFolder(tempFolder, targetFolder);
        Directory.Delete(tempFolder);

        var fileName = Path.Combine(targetFolder, $"{schematic}.json");
        var data = JsonSerializer.Deserialize<SchematicObjectDataList>(File.OpenRead(fileName));

        if (data == null)
        {
            return null;
        }

        data.Path = targetFolder;

        if (action != null)
        {
            AsyncUtilities.ExecuteOnMainThread(() => { action.Invoke(data); });
        }

        return data;
    }

    private static void CopyFolder(string folderA, string folderB)
    {
        if (!Directory.Exists(folderB))
        {
            Directory.CreateDirectory(folderB);
        }

        foreach (var file in Directory.GetFiles(folderA).Select(x => new FileInfo(x)))
        {
            var targetPath = Path.Combine(folderB, file.Name);

            // Delete the previous file before moving the new file
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }

            File.Move(file.FullName, targetPath);
        }
    }
}