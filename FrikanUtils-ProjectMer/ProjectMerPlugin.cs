using System;
using FrikanUtils.ProjectMer;
using HarmonyLib;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;

namespace FrikanUtils;

public class ProjectMerPlugin : Plugin
{
    public override string Name => "FrikanUtils-ProjectMer";
    public override string Description => "ProjectMer extension for FrikanUtils";
    public override string Author => "gamendegamer321";
    public override Version Version => new(1, 1, 0);
    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    public static ProjectMerPlugin Instance { get; private set; }

    private readonly Harmony _harmony = new("com.frikandelbroodje.utils-mer");

    public override void Enable()
    {
        Instance = this;
        _harmony.PatchAll();
        MerEventHandler.RegisterEvents();
    }

    public override void Disable()
    {
        _harmony.UnpatchAll();
        MerEventHandler.UnregisterEvents();
    }
}