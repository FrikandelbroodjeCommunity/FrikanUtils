using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Yaml;
using MapGeneration.Holidays;
using Utf8Json;

namespace FrikanUtils.FileSystem.Providers;

/// <summary>
/// The <see cref="BackupFileProvider"/> will allow plugins to register a URL to download the file from, when all other file providers fail.
/// It will only re-download the file when the plugin version is updated<br/>
/// <br/>
/// Filename and folder combinations must be unique to prevent conflicts with other plugins. Recommended to use something like <c>{folder}/{username}.{filename}.{extension}</c>.<br/>
/// <br/>
/// To update a file using the backup provider, the filename must be updated (e.g. by adding .v2 as part of the filename).
/// </summary>
public class BackupFileProvider : BaseFileProvider
{
    /// <inheritdoc/>
    public override string Name => "BackupFileProvider";
    
    /// <inheritdoc/>
    public override byte LoadPriority => 255;

    private const string DownloadDir = "AutoDownloads";

    private static readonly Dictionary<string, BackupInfo> Backups = new();
    private readonly HttpClient _httpClient = new();

    /// <summary>
    /// Disposes the HTTP client when shutting down.
    /// </summary>
    ~BackupFileProvider()
    {
        _httpClient.Dispose();
    }

    /// <inheritdoc/>
    public override async Task<string> SearchFullPath(string filename, string folder)
    {
        Logger.Debug($"Searching backup file provider for {folder}/{filename}",
            UtilitiesPlugin.PluginConfig.Debug);
        
        var combinedPath = Path.Combine(folder, filename);
        if (!Backups.TryGetValue(combinedPath, out var info))
        {
            Logger.Debug("No backup registered", UtilitiesPlugin.PluginConfig.Debug);
            return null;
        }
        
        var dirPath = Path.Combine(PathManager.Configs.FullName,
            Server.Port.ToString(),
            UtilitiesPlugin.Instance.Name,
            DownloadDir
        );

        if (!string.IsNullOrEmpty(folder))
        {
            dirPath = Path.Combine(dirPath, folder);
        }
        
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        foreach (var holidayFilename in GetHolidayFilenames(filename, info.AllowedHolidayTypes))
        {
            var filePath = Path.Combine(dirPath, $"{info.Version.Major}.{info.Version.Minor}.{info.Version.Build}.{info.Version.Revision}.{holidayFilename}");
            if (File.Exists(filePath))
            {
                Logger.Debug($"Found existing backup file {folder}/{holidayFilename}",
                    UtilitiesPlugin.PluginConfig.Debug);
                return filePath;
            }
        
            Logger.Info($"Attempting to download {folder}/{holidayFilename} ({info.Version}) from a backup URL");
            var response = await _httpClient.GetAsync(info.URL);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                continue;
            }
        
            File.WriteAllBytes(filePath, await response.Content.ReadAsByteArrayAsync());
            return filePath;
        }

        return null;
    }

    /// <inheritdoc/>
    public override async Task<T> SearchFile<T>(string filename, string folder, bool json)
    {
        var path = await SearchFullPath(filename, folder);
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        
        return json
            ? JsonSerializer.Deserialize<T>(File.OpenRead(path))
            : YamlConfigParser.Deserializer.Deserialize<T>(File.ReadAllText(path));
    }

    /// <summary>
    /// Register a backup URL for a required file for the plugin.
    /// If the file cannot be found elsewhere, it will be downloaded from the given URL.
    /// The filename and folder must match exactly for the backup to work.<br/>
    /// <br/>
    /// To change files during holidays, the holidays that are supported must be set in the <c>holidayTypes</c>.
    /// </summary>
    /// <param name="plugin">The instance of this plugin</param>
    /// <param name="filename">The name of the file</param>
    /// <param name="folder">The desired folder</param>
    /// <param name="url">The URL to download from</param>
    /// <param name="holidayTypes">The holiday types that are allowed</param>
    public static void RegisterBackup(Plugin plugin, string filename, string folder, string url, HolidayType[] holidayTypes = null)
        => RegisterBackup(plugin.Version, filename, folder, url, holidayTypes);

    /// <summary>
    /// Register a backup URL for a required file for the plugin.
    /// If the file cannot be found elsewhere, it will be downloaded from the given URL.
    /// The filename and folder must match exactly for the backup to work.<br/>
    /// <br/>
    /// To change files during holidays, the holidays that are supported must be set in the <c>holidayTypes</c>.
    /// </summary>
    /// <param name="version">The current version of the plugin</param>
    /// <param name="filename">The name of the file</param>
    /// <param name="folder">The desired folder</param>
    /// <param name="url">The URL to download from</param>
    /// <param name="holidayTypes">The holiday types that are allowed</param>
    public static void RegisterBackup(Version version, string filename, string folder, string url, HolidayType[] holidayTypes = null)
    {
        url = string.Format(url, version.Major, version.Minor, version.Build, version.Revision);
        Backups[Path.Combine(folder, filename)] = new BackupInfo(version, url, holidayTypes ?? []);
    }

    /// <summary>
    /// Generate a GitHub url to use as a backup file url.<br/>
    /// No spaces are allowed in the parameters.<br/>
    /// <br/>
    /// The version format will be used for the tag.
    /// {0} for the major version, {1} for the minor version, {2} for the build, {3} for the revision.
    /// Any undefined will automatically be set to -1.
    /// </summary>
    /// <param name="username">The username or organization name that owns the repository</param>
    /// <param name="repository">The name of the repository</param>
    /// <param name="filename">The name of the file, including extension</param>
    /// <param name="versionFormat">The format of the tag</param>
    /// <returns>Formatted GitHub URL</returns>
    public static string GetGithubUrl(string username, string repository, string filename,
        string versionFormat = "{0}.{1}.{2}")
        => $"https://github.com/{username}/{repository}/releases/download/{versionFormat}/{filename}";
    
    private struct BackupInfo(Version version, string url, HolidayType[] holidayTypes)
    {
        public readonly string URL = url;
        public readonly Version Version = version;
        public readonly HolidayType[] AllowedHolidayTypes = holidayTypes;
    }
}