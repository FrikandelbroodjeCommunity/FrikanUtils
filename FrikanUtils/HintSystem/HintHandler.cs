using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;

namespace FrikanUtils.HintSystem;

public static class HintHandler
{
    private static readonly List<Func<Player, string>> LobbyHints = [];
    private static readonly List<Func<Player, string>> GameHints = [];

    public static bool ForceDisableLobby;

    /// <summary>
    /// Add a function to add text to the lobby hints.
    /// Text will be added on top of the other hints.
    /// </summary>
    /// <param name="textFunc">Function to get the hint text</param>
    public static void AddLobbyText(Func<Player, string> textFunc)
    {
        LobbyHints.Add(textFunc);
    }

    /// <summary>
    /// Remove a function to no longer add the text to the lobby hints.
    /// </summary>
    /// <param name="textFunc">Function to get the hint text</param>
    public static void RemoveLobbyText(Func<Player, string> textFunc)
    {
        LobbyHints.Remove(textFunc);
    }

    /// <summary>
    /// Add a function to add text to the game hints.
    /// </summary>
    /// <param name="textFunc">Function to get the hint text</param>
    public static void AddGameText(Func<Player, string> textFunc)
    {
        GameHints.Add(textFunc);
    }

    /// <summary>
    /// Remove a function to no longer add the text to the game hints.
    /// </summary>
    /// <param name="textFunc">Function to get the hint text</param>
    public static void RemoveGameText(Func<Player, string> textFunc)
    {
        GameHints.Remove(textFunc);
    }

    internal static string GetLobbyText(Player ply, string color)
    {
        if (ForceDisableLobby)
        {
            return "";
        }

        var builder = new StringBuilder();
        builder.Append("\n\n\n\n\n\n\n\n\n\n<size=25>");

        foreach (var text in LobbyHints.Select(hint => hint.Invoke(ply)).Where(text => !string.IsNullOrEmpty(text)))
        {
            builder.AppendLine(text);
            builder.AppendLine();
        }

        builder.AppendLine(UtilitiesPlugin.PluginConfig.LobbyText);
        builder.Replace("<color=rainbow>", $"<color={color}>");

        return builder.ToString();
    }

    internal static string GetGameText(Player ply, string color)
    {
        var builder = new StringBuilder();
        builder.Append("\n\n\n\n\n\n\n<size=25>");

        var found = false;
        foreach (var text in GameHints.Select(hint => hint.Invoke(ply)).Where(text => !string.IsNullOrEmpty(text)))
        {
            found = true;
            builder.AppendLine(text);
            builder.AppendLine();
        }

        if (!found)
        {
            return null; // No text found, send back nothing
        }

        builder.Append("</size>");
        builder.Replace("<color=rainbow>", $"<color={color}>");

        return builder.ToString();
    }
}