using System;
using FrikanUtils.Npc.Enums;
using FrikanUtils.Npc.Patches;
using FrikanUtils.Utilities;
using LabApi.Features.Wrappers;
using Mirror;

namespace FrikanUtils.Npc;

public abstract class BaseNpc
{
    public Player Dummy { get; }

    public Action OnDestroy;
    public bool DestroyOnDeath = true;

    public BaseNpc(string name)
    {
        Dummy = NpcSystem.CreateHiddenDummy(name);
        Dummy.IsSpectatable = false;
        PlayerUtilities.RegisterNpc(Dummy);

        MaxMovementSpeedPatch.NpcModules.Add(Dummy.ReferenceHub);
    }

    public virtual void Destroy(DestroyReason reason)
    {
        OnDestroy?.Invoke();
        NetworkServer.Destroy(Dummy.GameObject);
    }
}