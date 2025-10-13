using System.IO;
using System.Threading.Tasks;
using LabApi.Loader;
using LabApi.Loader.Features.Yaml;
using Utf8Json;

namespace FrikanUtils.FileSystem;

/// <summary>
/// Provider for local files that are available on the drive of the server.
/// Will only search for files and folders that are in the <c>FrikanUtils</c> config folder.
/// </summary>
public class LocalFileProvider : BaseFileProvider
{
    /// <inheritdoc/>
    public override string Name => "LocalFileProvider";

    /// <inheritdoc/>
    public override Task<string> SearchFullPath(string filename, string folder)
    {
        var directory = string.IsNullOrEmpty(folder)
            ? UtilitiesPlugin.Instance.GetConfigDirectory().FullName
            : Path.Combine(UtilitiesPlugin.Instance.GetConfigDirectory().FullName, folder);

        foreach (var name in GetHolidayFilenames(filename))
        {
            var path = Path.Combine(directory, name);
            if (File.Exists(path))
            {
                return Task.FromResult(path);
            }
        }

        return Task.FromResult<string>(null);
    }

    /// <inheritdoc/>
    public override Task<T> SearchFile<T>(string filename, string folder, bool json) where T : class
    {
        var path = SearchFullPath(filename, folder).Result;
        if (string.IsNullOrEmpty(path))
        {
            return Task.FromResult<T>(null);
        }

        return Task.FromResult(json
            ? JsonSerializer.Deserialize<T>(File.OpenRead(path))
            : YamlConfigParser.Deserializer.Deserialize<T>(File.ReadAllText(path))
        );
    }
}