using System;
using FrikanUtils.Npc.Enums;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace FrikanUtils.Npc.Following;

/// <summary>
/// NPC that follows another player.
/// Functions the same as the dummy follow command from the base game, with the option to have a walk and sprint speed.
/// </summary>
public class FollowingNpc : BaseNpc
{
    /// <summary>
    /// The player that the NPC needs to follow.
    /// If it is set to <c>null</c> the NPC will halt.
    /// </summary>
    public Player TargetPlayer;

    /// <summary>
    /// If the NPC is this distance or closer from the <see cref="TargetPlayer"/>, it will execute the <see cref="ReachTargetAction"/>.
    /// </summary>
    public float IdleDistance = 2f;

    /// <summary>
    /// If the NPC is this distance or further from the <see cref="TargetPlayer"/>, it will start sprinting.
    /// </summary>
    public float SprintDistance = 7f;

    /// <summary>
    /// If the player is this distance or further from the <see cref="TargetPlayer"/>, it will execut ethe <see cref="OutOfRangeAction"/>.
    /// </summary>
    public float MaxDistance = 14f;

    /// <summary>
    /// The speed used when following a player between <see cref="IdleDistance"/> and <see cref="SprintDistance"/>.
    /// </summary>
    public float WalkSpeed = 3.9f;

    /// <summary>
    /// The speed used when following a player between <see cref="SprintDistance"/> and <see cref="MaxDistance"/>
    /// </summary>
    public float SprintSpeed = 5.4f;

    /// <summary>
    /// Executed when the player goes out of range of the NPC, and <see cref="OutOfRangeAction"/> is set to <see cref="OutOfRangeAction.CustomAction"/>.
    /// </summary>
    public Action OutsideOfRange;

    /// <summary>
    /// Executed when the NPC reaches the player, and <see cref="ReachTargetAction"/> is set to <see cref="ReachTargetAction.CustomAction"/>.
    /// </summary>
    public Action ReachedTarget;

    /// <summary>
    /// The action to execute when the NPC reaches the player (comes within <see cref="IdleDistance"/>).
    /// </summary>
    public ReachTargetAction ReachTargetAction = ReachTargetAction.Pause;

    /// <summary>
    /// The action to execute when the player goes out of range of the NPC (goes outside <see cref="MaxDistance"/>).
    /// </summary>
    public OutOfRangeAction OutOfRangeAction = OutOfRangeAction.Teleport;

    public NpcState State { get; internal set; } = NpcState.Paused;

    /// <summary>
    /// Create a new NPC with the given name and have it follow the target.
    /// </summary>
    /// <param name="name">The name of the dummy</param>
    /// <param name="target">The player to follow</param>
    public FollowingNpc(string name, Player target) : base(name)
    {
        Setup(target);
    }

    /// <summary>
    /// Make the given dummy a follow NPC and have it follow the given target.
    /// </summary>
    /// <param name="dummy">Dummy to make an NPC</param>
    /// <param name="target">The player to follow</param>
    public FollowingNpc(Player dummy, Player target) : base(dummy)
    {
        Setup(target);
    }

    private void Setup(Player target)
    {
        TargetPlayer = target;

        var component = Dummy.GameObject?.AddComponent<FollowingNpcComponent>();
        if (component == null)
        {
            Logger.Warn("Failed to add FollowingNpcComponent to npc!");
        }
        else
        {
            component.Data = this;
        }
    }
}