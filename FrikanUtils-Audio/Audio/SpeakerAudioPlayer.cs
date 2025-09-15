using LabApi.Features.Wrappers;
using UnityEngine;
using Utils.Networking;
using VoiceChat.Networking;

namespace FrikanUtils.Audio;

public class SpeakerAudioPlayer : AudioPlayerBase
{
    public override bool IsValid => Speaker.IsDestroyed;

    /// <summary>
    /// The toy that was spawned for this audio player.
    /// </summary>
    public readonly SpeakerToy Speaker;

    public SpeakerAudioPlayer(Vector3 position, Quaternion rotation)
    {
        Speaker = SpeakerToy.Create(position, rotation);

        var comp = Speaker.GameObject.AddComponent<AudioPlayerComponent>();
        comp.AudioPlayer = this;
    }

    public SpeakerAudioPlayer(Transform transform)
    {
        Speaker = SpeakerToy.Create(transform);

        var comp = Speaker.GameObject.AddComponent<AudioPlayerComponent>();
        comp.AudioPlayer = this;
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
}