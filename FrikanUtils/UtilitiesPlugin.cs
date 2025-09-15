using System;
using System.Collections.Generic;
using FrikanUtils.FileSystem;
using FrikanUtils.HintSystem;
using FrikanUtils.Keycard;
using FrikanUtils.ServerSpecificSettings;
using FrikanUtils.Utilities;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
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

    internal static UtilitiesPlugin Instance;
    internal static bool Debug => Instance.Config.Debug;
    internal static string[] RainbowTextColors => Instance.Config.RainbowTextColors;
    internal static float HintRefreshTime => Instance.Config.HintRefreshTime;
    internal static List<string> ServerSettingIds => Instance.Config.ServerSettingIds;
    internal static string NoSettingsList => Instance.Config.NoSettingsText;
    internal static bool ImprovedCardDetection => Instance.Config.ImprovedCardDetection;

    private GameObject _handlerObject;

    internal static void Save() => Instance.SaveConfig();

    public override void Enable()
    {
        Instance = this;

        // Register the default file provider
        FileHandler.RegisterProvider(new LocalFileProvider());

        ServerEvents.WaitingForPlayers += Reset;
        CustomKeycardEventHandler.RegisterEvents();
        SSSEventHandler.RegisterEvents();

        _handlerObject = new GameObject("FrikanUtils Handler Object");
        Object.DontDestroyOnLoad(_handlerObject);
        _handlerObject.AddComponent<RainbowKeycardHandler>();
        _handlerObject.AddComponent<HintSender>();
    }

    public override void Disable()
    {
        ServerEvents.WaitingForPlayers -= Reset;
        CustomKeycardEventHandler.UnregisterEvents();
        SSSEventHandler.UnregisterEvents();

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