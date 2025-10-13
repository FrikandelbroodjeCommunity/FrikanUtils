using System;
using FrikanUtils.Npc.Enums;
using UnityEngine;

namespace FrikanUtils.Npc;

/// <summary>
/// Base for all NPC components, informs the <see cref="BaseNpc"/> when the component or player gets destroyed.
/// </summary>
public abstract class BaseNpcComponent : MonoBehaviour
{
    /// <summary>
    /// Data for this NPC.
    /// </summary>
    public abstract BaseNpc NpcData { get; }

    private void OnDestroy()
    {
        NpcData.Destroy(DestroyReason.Cleanup);
    }
}