using System;
using FrikanUtils.ProjectMer;
using HarmonyLib;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;

namespace FrikanUtils;

/// <summary>
/// The plugin for the Project MER module.
/// </summary>
public class ProjectMerPlugin : Plugin
{
    /// <inheritdoc />
    public override string Name => "FrikanUtils-ProjectMer";

    /// <inheritdoc />
    public override string Description => "ProjectMer extension for FrikanUtils";

    /// <inheritdoc />
    public override string Author => "gamendegamer321";

    /// <inheritdoc />
    public override Version Version => new(UtilitiesPlugin.CurrentVersion);

    /// <inheritdoc />
    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

    /// <summary>
    /// Instance of the plugin.
    /// </summary>
    public static ProjectMerPlugin Instance { get; private set; }

    private readonly Harmony _harmony = new("com.frikandelbroodje.utils-mer");

    /// <inheritdoc />
    public override void Enable()
    {
        Instance = this;
        _harmony.PatchAll();
        MerEventHandler.RegisterEvents();
    }

    /// <inheritdoc />
    public override void Disable()
    {
        _harmony.UnpatchAll();
        MerEventHandler.UnregisterEvents();
    }
}