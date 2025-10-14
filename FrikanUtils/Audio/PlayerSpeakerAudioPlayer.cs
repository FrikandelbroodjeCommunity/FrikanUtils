using LabApi.Features.Wrappers;
using UnityEngine;
using Utils.Networking;
using VoiceChat.Networking;

namespace FrikanUtils.Audio;

/// <summary>
/// A speaker which is attached to a player, giving the illusion the audio is coming from the player.
/// </summary>
public class PlayerSpeakerAudioPlayer : SpeakerAudioPlayer
{
    /// <inheritdoc />
    public override bool IsValid => !Speaker.IsDestroyed;

    /// <summary>
    /// The player that the speaker is attached to.
    /// </summary>
    public readonly Player Player;

    /// <summary>
    /// Create a new player tracking speaker.
    /// </summary>
    /// <param name="player">Player that needs to be tracked by the speaker</param>
    public PlayerSpeakerAudioPlayer(Player player) : base(Vector3.zero, Quaternion.identity)
    {
        Player = player;

        var attach = Speaker.GameObject.AddComponent<AttachmentComponent>();
        attach.AudioPlayer = this;
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