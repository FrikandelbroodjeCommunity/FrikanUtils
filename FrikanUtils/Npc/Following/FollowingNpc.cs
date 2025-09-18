using System;
using FrikanUtils.Npc.Enums;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace FrikanUtils.Npc.Following;

public class FollowingNpc : BaseNpc
{
    public Player TargetPlayer;

    public float IdleDistance = 2f;
    public float SprintDistance = 7f;
    public float MaxDistance = 14f;

    public float WalkSpeed = 3.9f;
    public float SprintSpeed = 5.4f;

    public Action OutsideOfRange;
    public Action ReachedTarget;

    public ReachTargetAction ReachTargetAction = ReachTargetAction.Pause;
    public OutOfRangeAction OutOfRangeAction = OutOfRangeAction.Teleport;

    public FollowingNpc(Player target, string name) : base(name)
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