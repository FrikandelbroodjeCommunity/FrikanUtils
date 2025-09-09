using System;
using System.Collections.Generic;
using FrikanUtils.FileSystem;
using FrikanUtils.Utilities;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using MEC;

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
    internal static List<string> ServerSettingIds => Instance.Config.ServerSettingIds;
    internal static string NoSettingsList => Instance.Config.NoSettingsText;

    internal static void Save() => Instance.SaveConfig();

    public override void Enable()
    {
        Instance = this;

        // Register the default file provider
        FileHandler.RegisterProvider(new LocalFileProvider());

        ServerEvents.WaitingForPlayers += Reset;
    }

    public override void Disable()
    {
        ServerEvents.WaitingForPlayers -= Reset;
    }

    private static void Reset()
    {
        PlayerUtilities.BlacklistedPlayers.Clear();
        TeamUtilities.PlayerTeams.Clear();
    }
}