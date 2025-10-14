using System;
using AdminToys;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ProjectMer;

/// <summary>
/// Information about an interactable object in the world
/// </summary>
public struct Interactable
{
    /// <summary>
    /// The toy that is used to make the object interactable.
    /// </summary>
    public InvisibleInteractableToy Toy;

    /// <summary>
    /// The ID given when the interactable was made.
    /// </summary>
    public string Id;

    /// <summary>
    /// The function that is executed when a player starts interacting.
    ///
    /// Takes 2 arguments, the player that is interacting, and the <see cref="Id"/> of the interactable.
    /// Must return a boolean value for whether the player can interact with the interactable.
    /// </summary>
    public Func<Player, string, bool> PickingUp;

    /// <summary>
    /// The action that is executed when a player finished the interaction.
    ///
    /// Takes 2 arguments, the player that is interacting, and the <see cref="Id"/> of the interactable.
    /// </summary>
    public Action<Player, string> PickedUp;
}