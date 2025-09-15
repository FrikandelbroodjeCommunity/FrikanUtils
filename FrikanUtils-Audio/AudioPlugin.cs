using System;
using FrikanUtils.Audio;
using FrikanUtils.GlobalSettings;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;

namespace FrikanUtils;

public class AudioPlugin : Plugin<AudioConfig>
{
    public override string Name => "FrikanUtils-Audio";
    public override string Description => "Audio extension for FrikanUtils";
    public override string Author => "gamendegamer321";
    public override Version Version => new(1, 0, 0);
    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    public static AudioPlugin Instance { get; private set; }

    private readonly MuteSetting _muteSetting = new();
    private readonly VolumeSetting _volumeSetting = new();

    public override void Enable()
    {
        Instance = this;
        AudioSystem.RegisterEvents();
        
        GlobalSettingsHandler.RegisterSetting(_muteSetting);
        GlobalSettingsHandler.RegisterSetting(_volumeSetting);
    }

    public override void Disable()
    {
        AudioSystem.UnregisterEvents();
        
        GlobalSettingsHandler.UnregisterSetting(_muteSetting);
        GlobalSettingsHandler.UnregisterSetting(_volumeSetting);
    }
}