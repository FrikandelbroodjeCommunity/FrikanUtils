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

public class HubAudioPlayer : AudioPlayerBase
{
    public override bool IsValid => !Player.IsDestroyed;

    /// <summary>
    /// The player that was spawned for this audio player.
    /// </summary>
    public Player Player { get; private set; }

    /// <summary>
    /// The channel that the audio needs to be played to.
    /// </summary>
    public VoiceChatChannel TargetChannel = VoiceChatChannel.RoundSummary;

    public HubAudioPlayer(string botName = null, bool enableGodmode = true, bool canSpectate = false)
    {
        CreateDummy(botName, enableGodmode, canSpectate);
    }

    protected override void InternalCleanup()
    {
        NetworkServer.Destroy(Player.ReferenceHub.gameObject);
        PlayerUtilities.UnregisterNpc(Player);
    }

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

        PlayerUtilities.RegisterNpc(Player);

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