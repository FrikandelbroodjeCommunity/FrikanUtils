using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FrikanUtils.Utilities;
using LabApi.Features.Console;
using Utils.NonAllocLINQ;

namespace FrikanUtils.FileSystem;

/// <summary>
/// Handles getting paths and file from the file providers.
/// </summary>
public static class FileHandler
{
    private static readonly List<BaseFileProvider> FileProviders =
    [
        new LocalFileProvider()
    ];

    /// <summary>
    /// Register a file provider, only once registered a provider will be used.
    /// </summary>
    /// <param name="provider">Provider to register</param>
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
    /// </summary>
    /// <param name="filename">The name of the file</param>
    /// <param name="folder">The folder the file should be in</param>
    /// <param name="onResult">Executed on the main thread after a result is gotten</param>
    /// <returns>The full path to the file or null</returns>
    public static async Task<string> SearchFullPath(string filename, string folder = null,
        Action<string> onResult = null)
    {
        foreach (var provider in FileProviders)
        {
            try
            {
                var result = await provider.SearchFullPath(filename, folder);
                if (!string.IsNullOrEmpty(result))
                {
                    Logger.Debug($"Found full file path from provider {provider.Name}: {result}",
                        UtilitiesPlugin.PluginConfig.Debug);

                    if (onResult != null)
                    {
                        AsyncUtilities.ExecuteOnMainThread(() => onResult.Invoke(result));
                    }

                    return result;
                }

                Logger.Debug($"No full file path found in provider {provider.Name} for {folder}/{filename}",
                    UtilitiesPlugin.PluginConfig.Debug);
            }
            catch (Exception e)
            {
                Logger.Warn($"Encountered error while using provider: {provider}.\n{e}");
            }
        }

        Logger.Warn($"{filename} could not be found in {folder}");
        return null;
    }

    /// <summary>
    /// Use all file providers to try and Search for the file and convert it into the data as needed.
    /// Will return <c>null</c> if the file was not found, or it could not be parsed.
    /// This is an async method, allowing files to be downloaded and then parsed during execution.
    /// </summary>
    /// <param name="filename">The name of the file</param>
    /// <param name="folder">The folder the file should be in</param>
    /// <param name="json">Whether to read it as JSON or YAML</param>
    /// <param name="onResult">Executed on the main thread after a result is gotten</param>
    /// <typeparam name="T">The type the contents should be parsed to</typeparam>
    /// <returns>The file contents as <code>T</code>, or <code>default</code></returns>
    public static async Task<T> SearchFile<T>(string filename, string folder, bool json, Action<T> onResult = null)
        where T : class
    {
        foreach (var provider in FileProviders)
        {
            try
            {
                var result = await provider.SearchFile<T>(filename, folder, json);
                if (result != null)
                {
                    Logger.Debug($"Found file {folder}/{filename} from provider {provider.Name}",
                        UtilitiesPlugin.PluginConfig.Debug);

                    if (onResult != null)
                    {
                        AsyncUtilities.ExecuteOnMainThread(() => onResult.Invoke(result));
                    }

                    return result;
                }

                Logger.Debug($"No file found in provider {provider.Name} for {folder}/{filename}",
                    UtilitiesPlugin.PluginConfig.Debug);
            }
            catch (Exception e)
            {
                Logger.Warn($"Encountered error while using provider: {provider}.\n{e}");
            }
        }

        Logger.Warn($"{filename} could not be found in {folder}");
        return null;
    }
}