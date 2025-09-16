using System;
using System.Collections.Generic;
using System.Linq;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using RemoteAdmin.Communication;

namespace FrikanUtils.CustomDummyActions;

/// <summary>
/// Represents a module containing multiple actions displayed in the dummy menu.
/// </summary>
public abstract class DummyModuleBase : IEquatable<DummyModuleBase>
{
    /// <summary>
    /// Name of the module.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Check whether a player has permissions to view this module.
    /// </summary>
    /// <param name="player">The player to check permissions for</param>
    /// <returns>Whether the player has permission</returns>
    public virtual bool HasPermission(Player player) => true;

    /// <summary>
    /// Get the actions belonging to this module.
    /// The executor will be given in order to allow permission checks.
    /// Gives the target to allow certain actions to only show up for certain players.
    /// </summary>
    /// <param name="executor">Player that may execute the action</param>
    /// <param name="target">Target player for the action</param>
    /// <returns>All actions to display for this target and executor</returns>
    public abstract IEnumerable<ModuleAction> GetActions(Player executor, Player target);

    internal IEnumerable<string> GetStrings(Player executor, Player target)
    {
        yield return RaDummyActions.GroupPrefix + Name;
        foreach (var action in GetActions(executor, target))
        {
            yield return action.Name;
        }
    }

    internal ActionExecutable GetAction(Player executor, IEnumerable<Player> targets, string name)
    {
        var executable = new ActionExecutable
        {
            Players = []
        };

        foreach (var target in targets.Where(target => target != null))
        {
            var action = GetActions(executor, target).FirstOrDefault(x => x.Name.Replace(" ", "_") == name);
            if (action == null) continue;

            executable.Action ??= action.Action;
            executable.Players.Add(target);
        }

        return executable;
    }

    public bool Equals(DummyModuleBase other)
    {
        return Name == other.Name;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((DummyModuleBase)obj);
    }

    public override int GetHashCode()
    {
        return Name != null ? Name.GetHashCode() : 0;
    }
}