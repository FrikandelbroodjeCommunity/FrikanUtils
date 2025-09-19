using System;
using System.Collections.Generic;
using FrikanUtils.Npc.Enums;
using FrikanUtils.Npc.Patches;
using FrikanUtils.Utilities;
using LabApi.Features.Wrappers;
using Mirror;

namespace FrikanUtils.Npc;

public abstract class BaseNpc
{
    internal static readonly Dictionary<Player, BaseNpc> NpcsMapped = new();

    /// <summary>
    /// The dummy spawned for this NPC
    /// </summary>
    public Player Dummy { get; }

    /// <summary>
    /// The action that needs to be executed when the NPC gets destroyed.
    /// </summary>
    public Action<DestroyReason> OnDestroy;

    // NPC settings

    /// <summary>
    /// Whether the NPC should be destroyed when it dies
    /// </summary>
    public bool DestroyOnDeath = true;

    /// <summary>
    /// When enabled, this NPC will not count as a target for SCPs.
    /// It will also not be included when checking if the round has finished.
    /// </summary>
    public bool IgnoreTarget = true;

    /// <summary>
    /// When enabled, the NPC can safely look at SCP-096
    /// </summary>
    public bool IgnoreScp096 = true;

    /// <summary>
    /// When enabled, SCP-173 can continue walking when the NPC looks at it.
    /// </summary>
    public bool IgnoreScp173 = true;

    /// <summary>
    /// Whether the NPC has the ability to escape the facility.
    /// </summary>
    public bool CanEscape;

    public BaseNpc(string name)
    {
        Dummy = NpcSystem.CreateHiddenDummy(name);
        Dummy.IsSpectatable = false;
        NpcSystem.RegisterNpc(Dummy);

        MaxMovementSpeedPatch.NpcModules.Add(Dummy.ReferenceHub);
        NpcsMapped[Dummy] = this;
    }

    public virtual void Destroy(DestroyReason reason)
    {
        OnDestroy?.Invoke(reason);
        NpcsMapped.Remove(Dummy);
        NetworkServer.Destroy(Dummy.GameObject);
    }
}