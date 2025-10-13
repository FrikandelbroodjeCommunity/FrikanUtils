using System;
using FrikanUtils.GlobalSettings;
using FrikanUtils.HintSystem;
using FrikanUtils.Keycard;
using FrikanUtils.Npc;
using FrikanUtils.Npc.Patches;
using FrikanUtils.ServerSpecificSettings;
using FrikanUtils.Utilities;
using HarmonyLib;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FrikanUtils;

/// <summary>
/// The plugin for Frikan Utils.
/// </summary>
public class UtilitiesPlugin : Plugin<Config>
{
    /// <summary>
    /// The version of the current assembly.
    /// </summary>
    public const string CurrentVersion = "1.1.1";

    /// <inheritdoc />
    public override string Name => "FrikanUtils";

    /// <inheritdoc />
    public override string Description => "";

    /// <inheritdoc />
    public override string Author => "gamendegamer321";

    /// <inheritdoc />
    public override Version Version => new(CurrentVersion);

    /// <inheritdoc />
    public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;

    /// <inheritdoc />
    public override LoadPriority Priority => LoadPriority.Highest;

    /// <summary>
    /// The instance of the frikan utils plugin.
    /// </summary>
    public static UtilitiesPlugin Instance { get; private set; }

    internal static Config PluginConfig => Instance.Config;

    private GameObject _handlerObject;
    private readonly Harmony _harmony = new("com.frikandelbroodje.utils");

    internal static void Save() => Instance.SaveConfig();

    /// <inheritdoc />
    public override void Enable()
    {
        Instance = this;
        _harmony.PatchAll();

        // Register events
        ServerEvents.WaitingForPlayers += Reset;
        ServerEvents.RoundStarted += RoundStarted;
        CustomKeycardEventHandler.RegisterEvents();
        NpcEventHandler.RegisterEvents();
        if (Config.UseServerSpecificSettings) SSSEventHandler.RegisterEvents();

        // Register global settings
        GlobalSettingsHandler.RegisterMenus();

        // Register all monobehaviour handles
        _handlerObject = new GameObject("FrikanUtils Handler Object");
        _handlerObject.AddComponent<AsyncUtilities.AsyncUtilitiesComponent>();
        _handlerObject.AddComponent<RainbowKeycardHandler>();
        if (Config.UseHintSystem) _handlerObject.AddComponent<HintSender>();
        Object.DontDestroyOnLoad(_handlerObject);
    }

    /// <inheritdoc />
    public override void Disable()
    {
        _harmony.UnpatchAll();

        // Unregister events
        ServerEvents.WaitingForPlayers -= Reset;
        ServerEvents.RoundStarted -= RoundStarted;
        CustomKeycardEventHandler.UnregisterEvents();
        NpcEventHandler.UnregisterEvents();
        SSSEventHandler.UnregisterEvents();

        // Unregister global settings
        GlobalSettingsHandler.UnregisterMenus();

        // Delete all monobehaviour handles
        Object.Destroy(_handlerObject);
    }

    private static void Reset()
    {
        CustomKeycard.CustomKeycards.Clear();
        RainbowKeycardHandler.Keycards.Clear();
        NpcSystem.Npcs.Clear();
        TeamUtilities.PlayerTeams.Clear();
        MaxMovementSpeedPatch.NpcModules.Clear();
    }

    private static void RoundStarted()
    {
        HintHandler.ForceDisableLobby = false;
    }
}