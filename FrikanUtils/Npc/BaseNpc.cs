using System;
using FrikanUtils.Npc.Enums;
using FrikanUtils.Utilities;
using LabApi.Features.Wrappers;
using Mirror;

namespace FrikanUtils.Npc;

public abstract class BaseNpc
{
    public Player Npc { get; }

    public Action OnDestroy;
    public bool DestroyOnDeath;

    public BaseNpc(string name)
    {
        Npc = NpcSystem.CreateHiddenDummy(name);
        Npc.IsSpectatable = false;
        PlayerUtilities.RegisterNpc(Npc);
    }

    public virtual void Destroy(DestroyReason reason)
    {
        OnDestroy?.Invoke();
        NetworkServer.Destroy(Npc.GameObject);
    }
}