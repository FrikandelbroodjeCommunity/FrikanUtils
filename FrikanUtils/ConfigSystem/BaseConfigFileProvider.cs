using System.Threading.Tasks;
using FrikanUtils.FileSystem;

namespace FrikanUtils.ConfigSystem;

/// <summary>
/// Extension of the file provider that can give more information about the configuration that is being loaded.
///
/// When a config file provider is added, it will be prioritized over the other file providers.
/// Additionally, only the first config file provider will be used, or the first file provider that results in a file.
/// </summary>
public abstract class BaseConfigFileProvider : BaseFileProvider
{
    /// <inheritdoc />
    public override AllowedFileTypes FileTypes => AllowedFileTypes.Config;

    /// <summary>
    /// Uploads the current config as JSON with the current values.
    /// 
    /// After the config system has overriden the config, the new values will be included in this upload.
    /// </summary>
    /// <param name="pluginName">The name of the plugin</param>
    /// <param name="config">The current config</param>
    /// <returns>Task that indicates when it is complete</returns>
    public abstract Task UploadCurrentConfig(string pluginName, object config);
    
}