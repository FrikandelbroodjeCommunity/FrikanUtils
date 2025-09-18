using System;
using FrikanUtils.Npc.Enums;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace FrikanUtils.Npc.Following;

public class FollowingNpcComponent : BaseNpcComponent
{
    public override BaseNpc NpcData => Data;

    [NonSerialized] public FollowingNpc Data;

    private void Update()
    {
        if (!Data.Npc.IsAlive || Data.TargetPlayer == null) return;

        var distance = Vector3.Distance(Data.Npc.Position, Data.TargetPlayer.Position);
        if (distance >= Data.MaxDistance)
        {
            switch (Data.OutOfRangeAction)
            {
                case OutOfRangeAction.Pause:
                    break;
                case OutOfRangeAction.StopFollowing:
                    Data.TargetPlayer = null;
                    break;
                case OutOfRangeAction.Teleport:
                    Data.Npc.Position = Data.TargetPlayer.Position + Vector3.up * 0.1f;
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
            Move(Data.SprintSpeed);
        }
        else if (distance >= Data.IdleDistance)
        {
            Move(Data.WalkSpeed);
        }
        else
        {
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

    private void Move(float speed)
    {
        var direction = (Data.TargetPlayer.Position - Data.Npc.Position).normalized;
        ((IFpcRole)Data.Npc.RoleBase).FpcModule.CharController.Move(direction * (speed * Time.deltaTime));
    }
}