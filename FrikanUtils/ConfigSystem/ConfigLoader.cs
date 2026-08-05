using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FrikanUtils.FileSystem;
using FrikanUtils.Utilities;
using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Yaml;
using ServerOutput;

namespace FrikanUtils.ConfigSystem;

internal static class ConfigLoader
{
    private static bool _triggerRoundRestart;

    private static readonly MethodInfo SyncMethodInfo =
        typeof(ConfigLoader).GetMethod(nameof(SyncPluginConfig), BindingFlags.Static | BindingFlags.NonPublic);

    internal static async Task SyncConfigs()
    {
        Logger.Info("Attempting to sync configs");

        foreach (var plugin in PluginLoader.EnabledPlugins.Where(x =>
                     !UtilitiesPlugin.PluginConfig.ConfigSyncBlacklist.Contains(x.Name)))
        {
            try
            {
                var type = plugin.GetType();
                var configProperty = type.GetProperty(nameof(Plugin<>.Config));
                if (configProperty == null)
                {
                    Logger.Debug($"Skipped {plugin.Name}, no \"Config\" property", UtilitiesPlugin.PluginConfig.Debug);
                    continue;
                }

                Logger.Debug($"Attempting to sync config for {plugin.Name}", UtilitiesPlugin.PluginConfig.Debug);
                var config = configProperty.GetValue(plugin);

                if (config == null)
                {
                    Logger.Warn($"Failed to sync config for {plugin.Name}, no config object found");
                    continue;
                }

                var typedMethod = SyncMethodInfo.MakeGenericMethod(config.GetType());
                var task = typedMethod.Invoke(null, [plugin, config]);

                if (task is Task typedTask)
                {
                    await typedTask;
                }
            }
            catch (Exception e)
            {
                Logger.Warn($"Something went wrong while syncing config for {plugin.Name}.\n{e}");
            }
        }

        if (_triggerRoundRestart)
        {
            _triggerRoundRestart = false;

            AsyncUtilities.ExecuteOnMainThread(() =>
            {
                ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
                ServerConsole.AddOutputEntry(new ExitActionRestartEntry());
            });
        }
    }

    private static async Task SyncPluginConfig<T>(Plugin plugin, object config) where T : class, new()
    {
        if (plugin is not Plugin<T> typedPlugin)
        {
            Logger.Warn($"Plugin type did not match the expectation for plugin {plugin.Name}.");
            return;
        }

        Logger.Debug($"Syncing config for {plugin.Name}, type: {config.GetType().FullName}",
            UtilitiesPlugin.PluginConfig.Debug);

        foreach (var provider in FileHandler.FileProviders
                     .Where(x => x.FileTypes.HasFlag(AllowedFileTypes.Config))
                     .OrderByDescending(x => x is BaseConfigFileProvider)
                     .ThenBy(x => x.LoadPriority))
        {
            try
            {
                var result = await provider.SearchFile<T>($"{plugin.Name}.yml",
                    UtilitiesPlugin.PluginConfig.ConfigFolder, false);

                if (result == null)
                {
                    if (provider is BaseConfigFileProvider configProvider)
                    {
                        Logger.Debug(
                            $"No config found for {plugin.Name}, found a config file provider, uploading the config instead",
                            UtilitiesPlugin.PluginConfig.Debug);

                        await configProvider.UploadCurrentConfig(plugin.Name, config);
                        return;
                    }

                    Logger.Debug(
                        $"No config found for {plugin.Name} in provider {provider.Name} ({UtilitiesPlugin.PluginConfig.ConfigFolder}/{plugin.Name}.yml)",
                        UtilitiesPlugin.PluginConfig.Debug);
                    continue;
                }

                if (CompareClasses(result, config))
                {
                    return; // Config was not changed, so no need to update anything
                }

                // Update the config, some values may immediately update, but other require a server restart
                // Queue a soft restart on the next round end
                typedPlugin.Config = result;
                typedPlugin.SaveConfig();
                _triggerRoundRestart = true;
                return;
            }
            catch (Exception e)
            {
                Logger.Warn($"Encountered error while using provider, {provider}, for config.\n{e}");
            }
        }
    }

    private static bool CompareClasses<T>(T config, T newConfig) where T : class, new()
    {
        var serializer = YamlConfigParser.Serializer;
        return serializer.Serialize(config) == serializer.Serialize(newConfig);
    }
}