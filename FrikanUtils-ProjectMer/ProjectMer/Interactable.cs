using System;
using AdminToys;
using LabApi.Features.Wrappers;

namespace FrikanUtils.ProjectMer;

/// <summary>
/// Information about an interactable object in the world
/// </summary>
public struct Interactable
{
    public InvisibleInteractableToy Toy;
    public string Id;

    public Func<Player, string, bool> PickingUp;
    public Action<Player, string> PickedUp;
}