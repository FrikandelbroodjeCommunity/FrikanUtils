using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Yaml;
using Utf8Json;

namespace FrikanUtils.FileSystem;

/// <summary>
/// Provider to gather files from remote servers. Uses config to determine the URL.
/// </summary>
public class RemoteFileProvider : BaseFileProvider
{
    /// <inheritdoc />
    public override string Name => "RemoteFileProvider";

    private const string DownloadDir = "Downloads";
    private static string Url => UtilitiesPlugin.PluginConfig.RemoteFileProviderUrl;

    /// <summary>
    /// Searches for the file on the webserver and downloads it.
    /// The path of the downloaded file will be returned.
    /// Will return <c>null</c> if the file could not be downloaded.
    /// </summary>
    /// <inheritdoc/>
    public override async Task<string> SearchFullPath(string filename, string folder)
    {
        var contents = await DownloadContents(filename, folder);
        if (string.IsNullOrEmpty(contents))
        {
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

        var path = Path.Combine(dirPath, filename);
        File.WriteAllText(path, contents);
        return path;
    }

    /// <summary>
    /// Searches for the file on the webserver and downloads it.
    /// The parsed contents of the file will be returned.
    /// Will return <c>null</c> if the file could not be downloaded.
    /// </summary>
    /// <inheritdoc/>
    public override async Task<T> SearchFile<T>(string filename, string folder, bool json)
    {
        var contents = await DownloadContents(filename, folder);
        if (string.IsNullOrEmpty(contents))
        {
            return null;
        }

        return json ? JsonSerializer.Deserialize<T>(contents) : YamlConfigParser.Deserializer.Deserialize<T>(contents);
    }

    private static async Task<string> DownloadContents(string filename, string folder)
    {
        using var client = new HttpClient();

        if (string.IsNullOrEmpty(folder))
        {
            folder = "default";
        }

        filename = WebUtility.UrlEncode(filename);
        folder = WebUtility.UrlEncode(folder);

        var url = string.Format(Url, filename, folder);
        var response = await client.GetAsync(url);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return null;
        }

        return await response.Content.ReadAsStringAsync();
    }
}