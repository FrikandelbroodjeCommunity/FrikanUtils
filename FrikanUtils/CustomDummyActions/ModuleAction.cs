using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;

namespace FrikanUtils.CustomDummyActions;

/// <summary>
/// Represents an action within a module.
/// </summary>
public class ModuleAction
{
    /// <summary>
    /// The name of this action.
    /// This will be displayed as the name of the action in the RA.
    /// </summary>
    public string Name;

    /// <summary>
    /// The action that needs to be executed for this action.
    /// Will give the executing player and the players it is executed upon as arguments.
    /// Expects a string result to display to the executor.
    /// </summary>
    public Func<Player, List<Player>, string> Action;

    /// <summary>
    /// Create a new module action with the given information.
    /// </summary>
    /// <param name="name">Name of the action</param>
    /// <param name="action">Action to execute when selected by the player</param>
    public ModuleAction(string name, Func<Player, List<Player>, string> action)
    {
        Name = name;
        Action = action;
    }
}