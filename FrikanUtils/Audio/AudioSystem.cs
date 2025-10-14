using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerRoles;
using Utils.NonAllocLINQ;

namespace FrikanUtils.Audio;

public static class AudioSystem
{
    public static readonly List<AudioPlayerBase> AudioPlayers = [];

    internal static readonly Dictionary<string, bool> MusicPreference = new();
    internal static readonly List<int> DefaultFilterList = [];

    /// <summary>
    /// Check if the given player is used as an audio bot.
    /// </summary>
    /// <param name="player">Player to check for</param>
    /// <returns>Whether the player is used as a bot</returns>
    public static bool IsMusicBot(Player player)
    {
        foreach (var audioPlayer in AudioPlayers)
        {
            if (audioPlayer is HubAudioPlayer hubPlayer && hubPlayer.Player == player)
            {
                return true;
            }
        }

        return false;
    }

    internal static void UpdateVolumes()
    {
        foreach (var player in AudioPlayers)
        {
            // Update by setting the override value again
            player.OverrideVolume = player.OverrideVolume;
        }
    }

    internal static void SetUserPreference(Player ply, bool _, bool preference)
    {
        MusicPreference[ply.UserId] = preference;

        // If the preference is set to disabled, or the user is tutorial, mute the music
        if (preference || ply.Role == RoleTypeId.Tutorial)
        {
            RemoverUser(ply.PlayerId);
        }
        else
        {
            AddUser(ply);
        }
    }

    internal static void AddUser(Player ply)
    {
        if (IsMuted(ply)) return;
        DefaultFilterList.AddIfNotContains(ply.PlayerId);
    }

    internal static void RemoverUser(int id)
    {
        DefaultFilterList.Remove(id);
    }
    
    private static bool IsMuted(Player player)
    {
        // Unauthenticated users and dummys are muted by default
        if (player.UserId == null || player.IsDummy)
        {
            return true;
        }

        return MusicPreference.TryGetValue(player.UserId, out var value) && value;
    }
}