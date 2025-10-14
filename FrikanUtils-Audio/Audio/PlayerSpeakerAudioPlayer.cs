using LabApi.Features.Wrappers;
using UnityEngine;
using Utils.Networking;
using VoiceChat.Networking;

namespace FrikanUtils.Audio;

/// <summary>
/// A <see cref="SpeakerAudioPlayer"/> that follows the players.
/// Can make it appear like the audio is coming from the player.
/// </summary>
public class PlayerSpeakerAudioPlayer : SpeakerAudioPlayer
{
    /// <summary>
    /// The player that the speaker is attached to.
    /// </summary>
    public readonly Player Player;

    /// <summary>
    /// Create a new tracking speaker.
    /// </summary>
    /// <param name="player">The player the speaker needs to be attached to</param>
    public PlayerSpeakerAudioPlayer(Player player) : base(player.Position, Quaternion.identity)
    {
        Player = player;

        var comp = Speaker.GameObject.AddComponent<AudioPlayerComponent>();
        comp.AudioPlayer = this;

        var attach = Speaker.GameObject.AddComponent<AttachmentComponent>();
        attach.AudioPlayer = this;
    }

    /// <inheritdoc />
    protected override void InternalCleanup()
    {
        Speaker.Destroy();
    }

    /// <inheritdoc />
    protected internal override void SendMessage(byte[] data, int length)
    {
        var audioMessage = new AudioMessage(Speaker.ControllerId, data, length);
        audioMessage.SendToHubsConditionally(IsValidPlayer);
    }

    internal class AttachmentComponent : MonoBehaviour
    {
        public PlayerSpeakerAudioPlayer AudioPlayer;

        private void Update()
        {
            AudioPlayer.Speaker.Position = AudioPlayer.Player.Position;
        }
    }
}