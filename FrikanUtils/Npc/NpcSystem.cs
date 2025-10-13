using System;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using Mirror;
using NetworkManagerUtils.Dummies;
using Utils.NonAllocLINQ;

namespace FrikanUtils.Npc;

/// <summary>
/// Set of helper functions to make working with NPCs/Dummies easier.
/// </summary>
public static class NpcSystem
{
    internal static readonly List<Player> Npcs = [];

    /// <summary>
    /// Creates a normal dummy.
    /// Uses <see cref="DummyUtils.SpawnDummy"/> but converts it to a player before returning.
    /// </summary>
    /// <param name="name">Name of the dummy</param>
    /// <returns>Dummy player</returns>
    public static Player CreateDummy(string name) => Player.Get(DummyUtils.SpawnDummy(name));

    /// <summary>
    /// Creates a dummy that does not show up on the player list or in the remote admin.
    /// </summary>
    /// <param name="name">Name of the dummy</param>
    /// <param name="giveUserId">Whether the dummy should have a user id</param>
    /// <returns>Dummy player</returns>
    public static Player CreateHiddenDummy(string name, bool giveUserId = false)
    {
        var newPlayer = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
        var fakeConnection = new FakeConnection(giveUserId);
        var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
        hubPlayer.nicknameSync.MyNick = name;
        NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
        return Player.Get(hubPlayer);
    }

    /// <summary>
    /// Registers a player as an NPC, this will cause them to no longer be included the <see cref="GetPlayers"/>.
    /// </summary>
    /// <param name="npc">Player that is an NPC</param>
    public static void RegisterNpc(Player npc) => Npcs.AddIfNotContains(npc);

    /// <summary>
    /// Unregisters a player as an NPC, this will cause them to be included in the <see cref="GetPlayers"/> again.
    /// </summary>
    /// <param name="npc">Player that is no longer an NPC</param>
    public static void UnregisterNpc(Player npc) => Npcs.Remove(npc);

    private class FakeConnection : NetworkConnectionToClient
    {
        public readonly bool GiveUserId;

        public FakeConnection(bool giveUserId) : base(DummyNetworkConnection._idGenerator--)
        {
            GiveUserId = giveUserId;
        }

        public override string address => "localhost";

        public override void Send(ArraySegment<byte> segment, int channelId = 0)
        {
        }

        public override void Disconnect()
        {
        }
    }
}