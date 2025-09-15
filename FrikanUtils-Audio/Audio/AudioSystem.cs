using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerRoles;
using Utils.NonAllocLINQ;

namespace FrikanUtils.Audio;

public static class AudioSystem
{
    public static int GlobalVolume { get; internal set; }
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

    internal static void RegisterEvents()
    {
        PlayerEvents.ChangingRole += OnRoleChange;
        ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
    }

    internal static void UnregisterEvents()
    {
        PlayerEvents.ChangingRole -= OnRoleChange;
        ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
    }

    internal static void UpdateVolumes()
    {
        foreach (var player in AudioPlayers)
        {
            // Update by setting the override value again
            player.OverrideVolume = player.OverrideVolume;
        }
    }

    internal static void SetUserPreference(Player ply, bool preference)
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

    private static void OnWaitingForPlayers()
    {
        DefaultFilterList.Clear();
        AudioPlayers.RemoveAll(x => !x.IsValid);
    }

    private static void OnRoleChange(PlayerChangingRoleEventArgs ev)
    {
        if (ev.Player.IsPlayer)
        {
            if (ev.NewRole == RoleTypeId.Tutorial)
            {
                RemoverUser(ev.Player.PlayerId);
            }
            else if (ev.OldRole.RoleTypeId == RoleTypeId.Tutorial)
            {
                AddUser(ev.Player);
            }
        }

        // For NPCs check if it's one of the audio players, as they are not allowed to change role at round start.
        if (ev.Player.IsPlayer || ev.ChangeReason != RoleChangeReason.RoundStart) return;

        foreach (var audioPlayer in AudioPlayers)
        {
            if (audioPlayer is not HubAudioPlayer hubPlayer || hubPlayer.Player != ev.Player) continue;

            ev.IsAllowed = false;
            break;
        }
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