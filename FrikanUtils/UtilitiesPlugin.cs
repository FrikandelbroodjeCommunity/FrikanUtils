using System;
using FrikanUtils.FileSystem;
using FrikanUtils.GlobalSettings;
using FrikanUtils.HintSystem;
using FrikanUtils.Keycard;
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

public class UtilitiesPlugin : Plugin<Config>
{
    public override string Name => "FrikanUtils";
    public override string Description => "";
    public override string Author => "gamendegamer321";
    public override Version Version => new(1, 0, 0);
    public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;
    public override LoadPriority Priority => LoadPriority.Highest;

    public static UtilitiesPlugin Instance { get; private set; }

    internal static Config PluginConfig => Instance.Config;

    private GameObject _handlerObject;
    private readonly Harmony _harmony = new("com.frikandelbroodje.utils");

    internal static void Save() => Instance.SaveConfig();

    public override void Enable()
    {
        Instance = this;
        _harmony.PatchAll();

        // Register events
        ServerEvents.WaitingForPlayers += Reset;
        if (Config.UseKeycardImprovements) CustomKeycardEventHandler.RegisterEvents();
        if (Config.UseServerSpecificSettings) SSSEventHandler.RegisterEvents();

        // Register global settings
        InternalGlobalSettings.RegisterInternalSettings();

        // Register all monobehaviour handles
        _handlerObject = new GameObject("FrikanUtils Handler Object");
        _handlerObject.AddComponent<RainbowKeycardHandler>();
        if (Config.UseHintSystem) _handlerObject.AddComponent<HintSender>();
        Object.DontDestroyOnLoad(_handlerObject);
    }

    public override void Disable()
    {
        _harmony.UnpatchAll();

        // Register events
        ServerEvents.WaitingForPlayers -= Reset;
        CustomKeycardEventHandler.UnregisterEvents();
        SSSEventHandler.UnregisterEvents();

        // Register global settings
        InternalGlobalSettings.UnregisterInternalSettings();

        // Delete all monobehaviour handles
        Object.Destroy(_handlerObject);
    }

    private static void Reset()
    {
        CustomKeycard.CustomKeycards.Clear();
        RainbowKeycardHandler.Keycards.Clear();
        PlayerUtilities.BlacklistedPlayers.Clear();
        TeamUtilities.PlayerTeams.Clear();
    }
}