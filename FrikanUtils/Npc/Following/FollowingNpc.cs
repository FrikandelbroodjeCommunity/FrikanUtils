using System;
using FrikanUtils.Npc.Enums;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace FrikanUtils.Npc.Following;

public class FollowingNpc : BaseNpc
{
    public Player TargetPlayer;

    public float IdleDistance = 0.8f;
    public float SprintDistance = 16f;
    public float MaxDistance = 28f;

    public float WalkSpeed = 3.9f;
    public float SprintSpeed = 5.4f;

    public Action OutsideOfRange;
    public Action ReachedTarget;

    public ReachTargetAction ReachTargetAction = ReachTargetAction.Pause;
    public OutOfRangeAction OutOfRangeAction = OutOfRangeAction.Teleport;

    public FollowingNpc(Player target, string name) : base(name)
    {
        TargetPlayer = target;

        if (Npc.GameObject?.AddComponent<FollowingNpcComponent>() == null)
        {
            Logger.Warn("Failed to add FollowingNpcComponent to npc!");
        }
    }
}