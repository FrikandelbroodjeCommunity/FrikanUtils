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
    /// The toy that is used to detect interactions.
    /// </summary>
    public InvisibleInteractableToy Toy;

    /// <summary>
    /// The ID given when the interaction was made.
    /// </summary>
    public string Id;

    /// <summary>
    /// Action executed when the item pickup is started
    /// Takes 2 arguments, the player interacting, and the <see cref="Id"/>.
    /// Must return a bool value which tells the plugin whether the player is allowed to interact or not.
    /// </summary>
    public Func<Player, string, bool> PickingUp;

    /// <summary>
    /// Action executed when the item pickup is finished
    /// Takes 2 arguments, the player interacting, and the <see cref="Id"/>.
    /// </summary>
    public Action<Player, string> PickedUp;
}