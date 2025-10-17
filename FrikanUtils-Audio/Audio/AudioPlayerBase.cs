using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FrikanUtils.FileSystem;
using FrikanUtils.Utilities;
using LabApi.Features.Wrappers;

namespace FrikanUtils.Audio;

/// <summary>
/// Base for every audio player, contains the shared logic for all of them.
/// </summary>
public abstract class AudioPlayerBase
{
    /// <summary>
    /// Whether the player should loop the files its given.
    /// </summary>
    public bool Looping;

    /// <summary>
    /// Whether the player is currently paused.
    /// </summary>
    public bool Paused;

    /// <summary>
    /// The forced volume of the audio.
    /// If set to -1, the player will use the global volume controlled by the `ut v` command.
    /// </summary>
    public float OverrideVolume = -1;

    /// <summary>
    /// The volume the player should use. Will be the global volume or the <see cref="OverrideVolume"/>.
    /// </summary>
    public float Volume => OverrideVolume < 0 ? ConfigVolume : OverrideVolume;

    /// <summary>
    /// Get the default volume from the config.
    /// </summary>
    public abstract float ConfigVolume { get; }

    /// <summary>
    /// Whether the player is still valid, should be false after being cleaned up.
    /// </summary>
    public abstract bool IsValid { get; }

    /// <summary>
    /// Whether the players should be restricted to players that have audio enabled in their Server Specific Settings.
    /// When disabled, all players will be able to hear the audio.
    /// Each implementation should have a method of assigning a custom list if this is set to false!
    /// </summary>
    public bool RestrictPlayers;

    /// <summary>
    /// Filter to determine whether to play the audio for a player.
    /// This will only be used if <see cref="RestrictPlayers"/> is set to false.
    /// </summary>
    public Func<Player, bool> ValidPlayers;

    /// <summary>
    /// The files that are currently in the queue.
    /// </summary>
    public readonly List<string> QueuedFiles = [];

    /// <summary>
    /// The current position in the queue.
    /// </summary>
    public int CurrentPosition { get; internal set; }

    /// <summary>
    /// Whether the audio player is currently playing any sounds.
    /// </summary>
    public bool Playing { get; internal set; }

    internal bool BreakCurrentFile;

    /// <summary>
    /// Initialize the audio player, registers it.
    /// </summary>
    protected AudioPlayerBase()
    {
        AudioSystem.AudioPlayers.Add(this);
    }

    /// <summary>
    /// Add audio files to the queue.
    /// These will be loaded from the "Audio" folder on the plugin server,
    /// for each file it will attempt to apply a holiday to the file if needed.
    /// </summary>
    /// <param name="files">The full filenames to load</param>
    /// <param name="autoPlay">Whether to start playing immediately after the queue is loaded</param>
    public async Task Queue(IEnumerable<string> files, bool autoPlay)
    {
        foreach (var file in files)
        {
            var path = await FileHandler.SearchFullPath(file, "Audio");
            if (path != null)
            {
                // Chances of modifying at the same time are very low, but this makes sure nothing can go wrong by accident
                AsyncUtilities.ExecuteOnMainThread(() => QueueFile(path, -1));
            }
        }

        if (autoPlay)
        {
            Play(0);
        }
    }

    /// <summary>
    /// Start playing the queued audio files.
    /// </summary>
    /// <param name="queuePos">The position in the queue to start at</param>
    public void Play(int queuePos)
    {
        CurrentPosition = queuePos;
        Playing = true;
        BreakCurrentFile = true;
    }

    /// <summary>
    /// Add a file to the queue.
    /// </summary>
    /// <param name="path">Full path of the file</param>
    /// <param name="position">Position to add the file in the queue (-1 adds it at the end)</param>
    public void QueueFile(string path, int position)
    {
        if (position == -1)
        {
            QueuedFiles.Add(path);
        }
        else
        {
            QueuedFiles.Insert(position, path);
        }
    }

    /// <summary>
    /// Cleanup all data that belongs to this audio player
    /// </summary>
    public void Cleanup()
    {
        AudioSystem.AudioPlayers.Remove(this);
        InternalCleanup();
    }

    /// <summary>
    /// Stop playing the audio and will clear the queue.
    /// </summary>
    public void Stop()
    {
        Playing = false;
        BreakCurrentFile = true;
    }

    /// <summary>
    /// Function to clean up all data associated with this audio player.
    /// </summary>
    protected abstract void InternalCleanup();

    /// <summary>
    /// Method to check whether the given player is a valid player for sending audio to
    /// </summary>
    /// <param name="hub">referencehub of the player</param>
    /// <returns>Whether the player should receive audio</returns>
    protected bool IsValidPlayer(ReferenceHub hub)
    {
        var player = Player.Get(hub);
        if (player == null || !player.IsPlayer)
        {
            return false;
        }

        if (!RestrictPlayers)
        {
            return AudioSystem.DefaultFilterList.Contains(player.PlayerId);
        }

        return ValidPlayers == null || ValidPlayers.Invoke(player);
    }

    /// <summary>
    /// Send the actual audio packets.
    /// </summary>
    /// <param name="data">Audio data</param>
    /// <param name="length">Length of audio data</param>
    protected internal abstract void SendMessage(byte[] data, int length);
}