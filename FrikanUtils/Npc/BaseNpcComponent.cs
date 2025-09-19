using System;
using FrikanUtils.Npc.Enums;
using UnityEngine;

namespace FrikanUtils.Npc;

public abstract class BaseNpcComponent : MonoBehaviour
{
    public abstract BaseNpc NpcData { get; }

    private void OnDestroy()
    {
        NpcData.Destroy(DestroyReason.Cleanup);
    }
}