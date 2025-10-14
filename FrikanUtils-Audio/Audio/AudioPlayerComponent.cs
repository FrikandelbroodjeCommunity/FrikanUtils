using System;
using System.IO;
using NVorbis;
using UnityEngine;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;
using VoiceChat.Networking;
using Logger = LabApi.Features.Console.Logger;

namespace FrikanUtils.Audio;

/// <summary>
/// Component used to read the audio file and send the audio packets.
/// </summary>
public class AudioPlayerComponent : MonoBehaviour
{
    /// <summary>
    /// The audio player that controls this component and should be used for sending packets.
    /// </summary>
    [NonSerialized] public AudioPlayerBase AudioPlayer;

    private const int BufferSize = 11520;
    private const int SampleRate = 48000;
    private readonly PlaybackBuffer _playbackBuffer = new();
    private readonly OpusEncoder _encoder = new(OpusApplicationType.Voip);
    private VorbisReader _reader;

    private float _allowedSamples;
    private readonly float[] _sampleData = new float[BufferSize];
    private readonly byte[] _encodedData = new byte[512];

    private void Update()
    {
        if (AudioPlayer.BreakCurrentFile)
        {
            AudioPlayer.BreakCurrentFile = false;
            _reader.Dispose();
            _reader = null;
            return;
        }

        if (!AudioPlayer.Playing || AudioPlayer.Paused) return;

        if (_reader == null)
        {
            SelectNext();
            return;
        }

        _allowedSamples += Time.deltaTime * SampleRate;
        var count = _reader.ReadSamples(_sampleData, 0, Math.Min((int)_allowedSamples, BufferSize));
        if (count > 0)
        {
            for (var i = 0; i < count; i++)
            {
                _playbackBuffer.Write(_sampleData[i] * (AudioPlayer.Volume / 100f));
            }
        }
        else
        {
            SelectNext();
            _allowedSamples = 0;
            return;
        }

        _allowedSamples -= count;

        while (_playbackBuffer.Length >= BufferSize)
        {
            _playbackBuffer.ReadTo(_sampleData, 480);
            var length = _encoder.Encode(_sampleData, _encodedData);

            AudioPlayer.SendMessage(_encodedData, length);
        }
    }

    private void SelectNext()
    {
        _reader?.Dispose();
        _reader = null;

        var position = AudioPlayer.CurrentPosition;
        if (position < 0 || position >= AudioPlayer.Files.Count)
        {
            if (AudioPlayer.Looping) // Loop back to the start
            {
                AudioPlayer.CurrentPosition = 0;
            }
            else // No more files and we are not looping
            {
                AudioPlayer.Playing = false;
                AudioPlayer.Files.Clear();
                AudioPlayer.Stop();
                return;
            }
        }

        var path = AudioPlayer.Files[position];
        var reader = new VorbisReader(File.Open(path, FileMode.Open));
        AudioPlayer.CurrentPosition++;
        Logger.Info($"Created reader for {path}, total {reader.TotalSamples}");

        if (reader.Channels > 1)
        {
            Logger.Warn($"File {path} is stereo, music files need to be mono");
            return;
        }

        if (reader.SampleRate != 48000)
        {
            Logger.Warn($"Sample for {path} needs to be 48000");
            return;
        }

        _reader = reader;
    }

    private void OnDestroy()
    {
        _encoder.Dispose();
        _playbackBuffer.Dispose();
    }
}