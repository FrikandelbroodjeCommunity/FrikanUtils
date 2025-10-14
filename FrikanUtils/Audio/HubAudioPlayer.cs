using System;
using FrikanUtils.Npc;
using FrikanUtils.Utilities;
using LabApi.Features.Wrappers;
using MapGeneration.Holidays;
using MEC;
using Mirror;
using Utils.Networking;
using VoiceChat;
using VoiceChat.Networking;

namespace FrikanUtils.Audio;

/// <summary>
/// An audio player which plays audio through the mic of a dummy.
/// </summary>
public class HubAudioPlayer : AudioPlayerBase
{
    /// <inheritdoc />
    public override float ConfigVolume => UtilitiesPlugin.PluginConfig.AudioConfig.Volume;

    /// <inheritdoc />
    public override bool IsValid => !Player.IsDestroyed;

    /// <summary>
    /// The player that was spawned for this audio player.
    /// </summary>
    public Player Player { get; private set; }

    /// <summary>
    /// The channel that the audio needs to be played to.
    /// </summary>
    public VoiceChatChannel TargetChannel = VoiceChatChannel.RoundSummary;

    /// <summary>
    /// Create a new dummy and attach the audio player to that.
    /// </summary>
    /// <param name="botName">The desired name of the bot</param>
    /// <param name="enableGodmode">Whether the dummy should have godmode</param>
    /// <param name="canSpectate">Whether the dummy can be spectated</param>
    public HubAudioPlayer(string botName = null, bool enableGodmode = true, bool canSpectate = false)
    {
        CreateDummy(botName, enableGodmode, canSpectate);
    }

    /// <summary>
    /// Attach the audio player to the given dummy.
    /// </summary>
    /// <param name="player">Dummy to attach to</param>
    public HubAudioPlayer(Player player)
    {
        Player = player;

        NpcSystem.RegisterNpc(Player);

        var comp = Player.ReferenceHub.gameObject.AddComponent<AudioPlayerComponent>();
        comp.AudioPlayer = this;
    }

    /// <summary>
    /// Change the username of the dummy that plays the audio.
    /// </summary>
    /// <param name="name">New name for the dummy</param>
    public void SetUsername(string name)
    {
        try
        {
            Player.ReferenceHub.nicknameSync.SetNick(name ?? GetDefaultName());
        }
        catch (Exception)
        {
            // Ignore exception
        }
    }
    
    /// <inheritdoc />
    protected override void InternalCleanup()
    {
        NetworkServer.Destroy(Player.ReferenceHub.gameObject);
        NpcSystem.UnregisterNpc(Player);
    }
    
    /// <inheritdoc />
    protected internal override void SendMessage(byte[] data, int length)
    {
        var audioMessage = new VoiceMessage(Player.ReferenceHub, TargetChannel, data, length, false);
        audioMessage.SendToHubsConditionally(IsValidPlayer);
    }

    private void CreateDummy(string name, bool enableGodmode, bool canSpectate)
    {
        // Spawn a new (hidden) dummy
        Player = NpcSystem.CreateHiddenDummy(name ?? GetDefaultName());
        Player.IsSpectatable = canSpectate;

        NpcSystem.RegisterNpc(Player);

        // Auto enable GodMode on a small delay
        if (enableGodmode)
        {
            Timing.CallDelayed(Timing.WaitForOneFrame, () => { Player.IsGodModeEnabled = true; });
        }

        // Get the audio player
        var comp = Player.ReferenceHub.gameObject.AddComponent<AudioPlayerComponent>();
        comp.AudioPlayer = this;
    }

    private static string GetDefaultName()
    {
        if (HolidayType.Halloween.IsActive())
        {
            return UtilitiesPlugin.PluginConfig.AudioConfig.DefaultHalloweenName;
        }

        if (HolidayType.Christmas.IsActive())
        {
            return UtilitiesPlugin.PluginConfig.AudioConfig.DefaultChristmasName;
        }

        if (HolidayType.AprilFools.IsActive())
        {
            return UtilitiesPlugin.PluginConfig.AudioConfig.DefaultAprilFoolsName;
        }

        return UtilitiesPlugin.PluginConfig.AudioConfig.DefaultName;
    }
}