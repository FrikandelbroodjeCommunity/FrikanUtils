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
/// An audio player that plays the audio through the microphone of a dummy.
/// </summary>
public class HubAudioPlayer : AudioPlayerBase
{
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
    /// Create a new dummy and attach the audio player to it.
    /// </summary>
    /// <param name="botName">Name of the new dummy</param>
    /// <param name="enableGodmode">Whether the dummy has godmode enabled</param>
    /// <param name="canSpectate">Whether the dummy can be spectated</param>
    public HubAudioPlayer(string botName = null, bool enableGodmode = true, bool canSpectate = false)
    {
        CreateDummy(botName, enableGodmode, canSpectate);
    }

    /// <summary>
    /// Attach an audio player to the given dummy.
    /// </summary>
    /// <param name="player">Dummy to attach audio player to</param>
    public HubAudioPlayer(Player player)
    {
        Player = player;

        NpcSystem.RegisterNpc(Player);

        var comp = Player.ReferenceHub.gameObject.AddComponent<AudioPlayerComponent>();
        comp.AudioPlayer = this;
    }

    /// <summary>
    /// Change the username of the dummy.
    /// </summary>
    /// <param name="name">New name of the dummy</param>
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
            return AudioPlugin.Instance.Config.DefaultHalloweenName;
        }

        if (HolidayType.Christmas.IsActive())
        {
            return AudioPlugin.Instance.Config.DefaultChristmasName;
        }

        if (HolidayType.AprilFools.IsActive())
        {
            return AudioPlugin.Instance.Config.DefaultAprilFoolsName;
        }

        return AudioPlugin.Instance.Config.DefaultName;
    }
}