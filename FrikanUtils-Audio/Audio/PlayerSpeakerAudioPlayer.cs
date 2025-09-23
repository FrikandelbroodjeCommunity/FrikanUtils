using LabApi.Features.Wrappers;
using UnityEngine;
using Utils.Networking;
using VoiceChat.Networking;

namespace FrikanUtils.Audio;

public class PlayerSpeakerAudioPlayer : AudioPlayerBase
{
    public override bool IsValid => !Speaker.IsDestroyed;

    /// <summary>
    /// The player that the speaker is attached to.
    /// </summary>
    public readonly Player Player;

    /// <summary>
    /// The toy that was spawned for this audio player.
    /// </summary>
    public readonly SpeakerToy Speaker;

    public PlayerSpeakerAudioPlayer(Player player)
    {
        Player = player;
        Speaker = SpeakerToy.Create(player.Position, Quaternion.identity);

        var comp = Speaker.GameObject.AddComponent<AudioPlayerComponent>();
        comp.AudioPlayer = this;

        var attach = Speaker.GameObject.AddComponent<AttachmentComponent>();
        attach.AudioPlayer = this;
    }

    protected override void InternalCleanup()
    {
        Speaker.Destroy();
    }

    protected internal override void SendMessage(byte[] data, int length)
    {
        var audioMessage = new AudioMessage(Speaker.ControllerId, data, length);
        audioMessage.SendToHubsConditionally(IsValidPlayer);
    }

    public class AttachmentComponent : MonoBehaviour
    {
        public PlayerSpeakerAudioPlayer AudioPlayer;

        private void Update()
        {
            AudioPlayer.Speaker.Position = AudioPlayer.Player.Position;
        }
    }
}