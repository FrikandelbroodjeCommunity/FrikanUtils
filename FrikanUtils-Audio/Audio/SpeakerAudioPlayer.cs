using LabApi.Features.Wrappers;
using UnityEngine;
using Utils.Networking;
using VoiceChat.Networking;

namespace FrikanUtils.Audio;

/// <summary>
/// Plays audio through an invisible speaker.
/// </summary>
public class SpeakerAudioPlayer : AudioPlayerBase
{
    /// <inheritdoc />
    public override float ConfigVolume => AudioPlugin.Instance.Config.VolumeSpeaker;

    /// <inheritdoc />
    public override bool IsValid => !Speaker.IsDestroyed;

    /// <summary>
    /// The toy that was spawned for this audio player.
    /// </summary>
    public readonly SpeakerToy Speaker;

    /// <summary>
    /// Create a new speaker at the given position using the given rotation.
    /// </summary>
    /// <param name="position">Position to spawn at</param>
    /// <param name="rotation">Rotation to spawn with</param>
    public SpeakerAudioPlayer(Vector3 position, Quaternion rotation)
    {
        Speaker = SpeakerToy.Create(position, rotation);

        var comp = Speaker.GameObject.AddComponent<AudioPlayerComponent>();
        comp.AudioPlayer = this;
    }

    /// <summary>
    /// Create a new speaker attached to the given transform.
    /// </summary>
    /// <param name="transform">Transform to attach to</param>
    public SpeakerAudioPlayer(Transform transform)
    {
        Speaker = SpeakerToy.Create(transform);

        var comp = Speaker.GameObject.AddComponent<AudioPlayerComponent>();
        comp.AudioPlayer = this;
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
}