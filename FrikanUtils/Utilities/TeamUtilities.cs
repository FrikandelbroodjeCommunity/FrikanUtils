using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Wrappers;
using PlayerRoles;
using UnityEngine;
using Utils.NonAllocLINQ;
using Logger = LabApi.Features.Console.Logger;
using Random = System.Random;

namespace FrikanUtils.Utilities;

public static class TeamUtilities
{
    /// <summary>
    /// Contains the user id mapped to the team the player was added to.
    /// This is done so, even after a player dies, it is still possible to know the team they were a part of.
    /// </summary>
    public static readonly Dictionary<string, RoleTypeId> PlayerTeams = new();

    public static readonly RoleTypeId[] TeamRoles =
    [
        RoleTypeId.ClassD,
        RoleTypeId.Scientist,
        RoleTypeId.NtfCaptain,
        RoleTypeId.ChaosRifleman,
        RoleTypeId.FacilityGuard
    ];

    private static readonly Random Random = new();

    /// <summary>
    /// Assign all players (gotten from <see cref="PlayerUtilities.GetPlayers"/>) into random teams.
    /// Each team will be teleported to their spawn point, as such there will be as many teams as spawn points.
    /// The teams are limited by the amount of visually distinct human roles there are,
    /// so <b>there can be a maximum of 5 teams</b>
    /// </summary>
    /// <param name="spawnPoints"></param>
    public static void AssignEqualTeams(Vector3[] spawnPoints)
    {
        if (spawnPoints.Length > TeamRoles.Length)
        {
            Logger.Error($"Cannot assign more teams than are available {spawnPoints.Length} > {TeamRoles.Length}");
            return;
        }

        var players = PlayerUtilities.GetPlayers().ToList();
        var currentTeam = 0;
        while (players.Count > 0)
        {
            var randomPlayer = players[Random.Next(players.Count)];
            players.Remove(randomPlayer);

            randomPlayer.SetRole(TeamRoles[currentTeam], flags: RoleSpawnFlags.None);
            randomPlayer.Position = spawnPoints[currentTeam];

            PlayerTeams[randomPlayer.UserId] = TeamRoles[currentTeam];

            currentTeam++;
            currentTeam %= spawnPoints.Length;
        }
    }

    /// <summary>
    /// Get all players that are a part of a certain team.
    /// Will return an empty array if the team is not in use.
    /// </summary>
    /// <param name="team">The team role to get the players for</param>
    /// <returns>The players on this team as an array</returns>
    public static Player[] GetPlayersOnTeam(RoleTypeId team)
    {
        return Player.List
            .Where(x => PlayerTeams.TryGetValue(x.UserId, out var playerTeam) && playerTeam == team)
            .ToArray();
    }

    /// <summary>
    /// Will see if any of the teams has one (is the last one standing).
    /// If so it will return true and the <paramref name="teamRole"/> will be set to the team that won.
    /// If more than 1 team is still alive, it will return false.
    /// <br/>
    /// <i>If there are no teams, it will return true with <paramref name="teamRole"/> set to <see cref="RoleTypeId.None"/>.</i>
    /// </summary>
    /// <param name="teamRole">The winning team role or <see cref="RoleTypeId.None"/></param>
    /// <returns>Whether a team has won</returns>
    public static bool TryGetWinningTeam(out RoleTypeId teamRole)
    {
        var teams = new List<RoleTypeId>();

        foreach (var player in Player.List)
        {
            if (PlayerTeams.TryGetValue(player.UserId, out var team) && player.Role == team)
            {
                teams.AddIfNotContains(team);

                if (teams.Count > 1)
                {
                    teamRole = RoleTypeId.None;
                    return false;
                }
            }
        }

        if (teams.Count == 0)
        {
            teamRole = RoleTypeId.None;
            return true;
        }

        teamRole = teams[0];
        return true;
    }
}