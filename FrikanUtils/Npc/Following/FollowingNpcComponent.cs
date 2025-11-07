using System;
using FrikanUtils.Npc.Enums;
using PlayerRoles.FirstPersonControl;
using RelativePositioning;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace FrikanUtils.Npc.Following;

internal class FollowingNpcComponent : BaseNpcComponent
{
    public override BaseNpc NpcData => Data;

    [NonSerialized] public FollowingNpc Data;

    private void Update()
    {
        if (!Data.Dummy.IsAlive || Data.TargetPlayer == null) return;
        if (Data.Dummy.RoleBase is not IFpcRole fpcRole) return;

        var dir = Data.TargetPlayer.Position - Data.Dummy.Position;
        fpcRole.FpcModule.MouseLook.LookAtDirection(dir);

        var distance = Vector3.Distance(Data.Dummy.Position, Data.TargetPlayer.Position);
        if (distance >= Data.MaxDistance)
        {
            Data.State = NpcState.Paused;
            switch (Data.OutOfRangeAction)
            {
                case OutOfRangeAction.Pause:
                    break;
                case OutOfRangeAction.StopFollowing:
                    Data.TargetPlayer = null;
                    break;
                case OutOfRangeAction.Teleport:
                    Data.Dummy.Position = Data.TargetPlayer.Position + Vector3.up * 0.1f;
                    break;
                case OutOfRangeAction.Destroy:
                    Data.Destroy(DestroyReason.OutsideOfRange);
                    break;
                case OutOfRangeAction.CustomAction:
                    Data.OutsideOfRange?.Invoke();
                    break;
                default:
                    Logger.Warn($"OutOfRangeAction {Data.OutOfRangeAction} is not supported on FollowingNpcs!");
                    break;
            }
        }
        else if (distance >= Data.SprintDistance)
        {
            Data.State = NpcState.Sprinting;
            Move(fpcRole, dir, Data.SprintSpeed);
        }
        else if (distance >= Data.IdleDistance)
        {
            Data.State = NpcState.Walking;
            Move(fpcRole, dir, Data.WalkSpeed);
        }
        else
        {
            Data.State = NpcState.Paused;
            switch (Data.ReachTargetAction)
            {
                case ReachTargetAction.Pause:
                    break;
                case ReachTargetAction.Destroy:
                    Data.Destroy(DestroyReason.ReachedTarget);
                    break;
                case ReachTargetAction.CustomAction:
                    Data.ReachedTarget?.Invoke();
                    break;
                default:
                    Logger.Warn($"ReachTargetAction {Data.ReachTargetAction} is not supported on FollowingNpcs!");
                    break;
            }
        }
    }

    private void Move(IFpcRole fpcRole, Vector3 dir, float speed)
    {
        var vector3 = Time.deltaTime * speed * dir.normalized;
        fpcRole.FpcModule.Motor.ReceivedPosition = new RelativePosition(Data.Dummy.Position + vector3);
    }
}