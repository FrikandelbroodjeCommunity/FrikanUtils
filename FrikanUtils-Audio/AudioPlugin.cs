using System;
using FrikanUtils.Audio;
using FrikanUtils.GlobalSettings;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;

namespace FrikanUtils;

/// <summary>
/// The plugin for the audio module.
/// </summary>
public class AudioPlugin : Plugin<AudioConfig>
{
    /// <inheritdoc />
    public override string Name => "FrikanUtils-Audio";

    /// <inheritdoc />
    public override string Description => "Audio extension for FrikanUtils";

    /// <inheritdoc />
    public override string Author => "gamendegamer321";

    /// <inheritdoc />
    public override Version Version => new(UtilitiesPlugin.CurrentVersion);

    /// <inheritdoc />
    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    /// <summary>
    /// Instance of the plugin.
    /// </summary>
    public static AudioPlugin Instance { get; private set; }

    private readonly MuteSetting _muteSetting = new();
    private readonly VolumeSetting _volumeSetting = new();

    /// <inheritdoc />
    public override void Enable()
    {
        Instance = this;
        AudioSystem.RegisterEvents();

        GlobalSettingsHandler.RegisterSetting(_muteSetting);
        GlobalSettingsHandler.RegisterSetting(_volumeSetting);
    }

    /// <inheritdoc />
    public override void Disable()
    {
        AudioSystem.UnregisterEvents();

        GlobalSettingsHandler.UnregisterSetting(_muteSetting);
        GlobalSettingsHandler.UnregisterSetting(_volumeSetting);
    }
}