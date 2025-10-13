using System.Linq;
using System.Text;
using CommandSystem;
using LabApi.Features.Wrappers;
using RemoteAdmin;

namespace FrikanUtils.Utilities;

/// <summary>
/// Set of utilities to help with the execution of commands.
/// </summary>
public static class CommandUtilities
{
    /// <summary>
    /// Try to get the player belonging to this command sender.
    /// If this his not a player command sender, or the player was not found it will return false and give the failure reason in the response.
    /// If it was successful it will give the player and return true.
    /// </summary>
    /// <param name="sender">Command sender</param>
    /// <param name="player">Found player or null</param>
    /// <param name="response">Failure reason or null</param>
    /// <returns>Whether a player command sender was found</returns>
    public static bool TryGetPlayer(this ICommandSender sender, out Player player, out string response)
    {
        player = null;
        if (sender is not PlayerCommandSender playerSender)
        {
            response = "This command can only be executed by a player.";
            return false;
        }

        player = Player.Get(playerSender);
        if (player == null)
        {
            response = "Something unexpected went wrong.";
            return false;
        }

        response = null;
        return true;
    }

    /// <summary>
    /// Try to find the matching player to the given text. Will search by ID if an integer is given.
    /// Otherwise, it will attempt to search by name.
    /// </summary>
    /// <param name="text">Text to search for</param>
    /// <param name="player">Found player or null</param>
    /// <param name="response">Failure reason or null</param>
    /// <returns>Whether a player was found</returns>
    public static bool TryFindPlayer(string text, out Player player, out string response)
    {
        player = null;
        response = null;

        // Search using ID
        if (int.TryParse(text, out var id))
        {
            player = Player.List.First(x => x.PlayerId == id);

            if (player != null) return true;

            response = $"No player with the ID {id} was found!";
            return false;
        }

        // Search using player name
        if (Player.TryGetPlayersByName(text, out var players))
        {
            if (players.Count > 1)
            {
                var builder = new StringBuilder();
                builder.AppendLine("The search was ambiguous, the following players match:");

                foreach (var ply in players)
                {
                    builder.AppendLine($" - ({ply.PlayerId}) {ply.DisplayName}");
                }

                response = builder.ToString();
                return false;
            }

            player = players[0];
            return true;
        }

        response = $"No players currently in the server have \"{text}\" in their name!";
        return false;
    }
}