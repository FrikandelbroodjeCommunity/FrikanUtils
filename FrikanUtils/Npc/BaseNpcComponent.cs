using UnityEngine;

namespace FrikanUtils.Npc;

public abstract class BaseNpcComponent : MonoBehaviour
{
    public abstract BaseNpc NpcData { get; }
}