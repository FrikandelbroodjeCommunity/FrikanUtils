using System.Collections.Generic;
using System.Threading.Tasks;
using LabApi.Features.Console;
using Utils.NonAllocLINQ;

namespace FrikanUtils.FileSystem;

public static class FileHandler
{
    private static readonly List<BaseFileProvider> FileProviders =
    [
        new LocalFileProvider()
    ];

    public static void RegisterProvider(BaseFileProvider provider)
    {
        if (FileProviders.Contains(provider))
        {
            Logger.Warn($"File provider {provider.Name} is already registered");
            return;
        }

        FileProviders.AddIfNotContains(provider);
        Logger.Debug($"Registered file provider {provider.Name}", UtilitiesPlugin.PluginConfig.Debug);
    }

    /// <summary>
    /// Use all file providers to search for a file on the disk.
    /// This is an async method, allowing files to be downloaded and written to the drive during execution.
    ///
    /// This also searches for special "event" files. (e.g. "Halloween-{filename}")
    /// </summary>
    /// <param name="filename">The name of the file</param>
    /// <param name="folder">The folder the file should be in</param>
    /// <returns>The full path to the file or null</returns>
    public static async Task<string> SearchFullPath(string filename, string folder = null)
    {
        foreach (var provider in FileProviders)
        {
            var result = await provider.SearchFullPath(filename, folder);
            if (!string.IsNullOrEmpty(result))
            {
                Logger.Debug($"Found full file path from provider {provider.Name}: {result}",
                    UtilitiesPlugin.PluginConfig.Debug);
                return result;
            }

            Logger.Debug($"No full file path found in provider {provider.Name} for {folder}/{filename}",
                UtilitiesPlugin.PluginConfig.Debug);
        }

        Logger.Warn($"{filename} could not be found in {folder}");
        return null;
    }

    /// <summary>
    /// Use all file providers to try and Search for the file and convert it into the data as needed.
    /// Will return <code>default</code> if the file was not found, or it could not be parsed.
    /// This is an async method, allowing files to be downloaded and then parsed during execution.
    /// 
    /// This also searches for special "event" files. (e.g. "Halloween-{filename}")
    /// </summary>
    /// <param name="filename">The name of the file</param>
    /// <param name="folder">The folder the file should be in</param>
    /// <param name="json">Whether to read it as JSON or YAML</param>
    /// <typeparam name="T">The type the contents should be parsed to</typeparam>
    /// <returns>The file contents as <code>T</code>, or <code>default</code></returns>
    public static async Task<T> SearchFile<T>(string filename, string folder, bool json)
    {
        foreach (var provider in FileProviders)
        {
            var result = await provider.SearchFile<T>(filename, folder, json);
            if (!result.Equals(default(T)))
            {
                Logger.Debug($"Found file {folder}/{filename} from provider {provider.Name}",
                    UtilitiesPlugin.PluginConfig.Debug);
                return result;
            }

            Logger.Debug($"No file found in provider {provider.Name} for {folder}/{filename}",
                UtilitiesPlugin.PluginConfig.Debug);
        }

        Logger.Warn($"{filename} could not be found in {folder}");
        return default;
    }
}