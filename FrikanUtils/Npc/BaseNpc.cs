using System;
using System.Collections.Generic;
using FrikanUtils.Npc.Enums;
using FrikanUtils.Npc.Patches;
using FrikanUtils.Utilities;
using LabApi.Features.Wrappers;
using Mirror;

namespace FrikanUtils.Npc;

/// <summary>
/// Base for all NPCs, contains all information that are required for all NPCs and some basic logic for destroying NPCs.
/// </summary>
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
    /// When enabled, this NPC will be completely ignored during the check for if there is a last player alive.
    /// </summary>
    public bool IgnoreLastTarget
    {
        get => NpcSystem.IgnoreHumanTarget.Contains(Dummy);
        set
        {
            if (value)
            {
                NpcSystem.AddIgnoreHumanTarget(Dummy);
            }
            else
            {
                NpcSystem.RemoveIgnoreHumanTarget(Dummy);
            }
        }
    }

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

    /// <summary>
    /// Create a new dummy for this NPC with the given name.
    /// </summary>
    /// <param name="name">Name of the new dummy</param>
    public BaseNpc(string name) : this(NpcSystem.CreateHiddenDummy(name))
    {
    }

    /// <summary>
    /// Use the given dummy for this NPC.
    /// </summary>
    /// <param name="dummy">Dummy to use for this NPC</param>
    public BaseNpc(Player dummy)
    {
        Dummy = dummy;
        Dummy.IsSpectatable = false;
        NpcSystem.RegisterNpc(Dummy);
        NpcSystem.AddIgnoreHumanTarget(Dummy);

        MaxMovementSpeedPatch.NpcModules.Add(Dummy.ReferenceHub);
        NpcsMapped[Dummy] = this;
    }

    /// <summary>
    /// Destroys this NPC with the given reason.
    /// </summary>
    /// <param name="reason">The reason why the NPC was destroyed</param>
    public virtual void Destroy(DestroyReason reason)
    {
        OnDestroy?.Invoke(reason);
        NpcsMapped.Remove(Dummy);
        NetworkServer.Destroy(Dummy.GameObject);
    }
}