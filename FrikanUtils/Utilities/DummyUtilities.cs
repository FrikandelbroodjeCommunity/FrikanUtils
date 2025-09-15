using System;
using LabApi.Features.Wrappers;
using Mirror;
using NetworkManagerUtils.Dummies;

namespace FrikanUtils.Utilities;

public static class DummyUtilities
{
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
    /// <returns>Dummy player</returns>
    public static Player CreateHiddenDummy(string name)
    {
        var newPlayer = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
        var fakeConnection = new FakeConnection();
        var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
        hubPlayer.nicknameSync.MyNick = name;
        NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
        return Player.Get(hubPlayer);
    }

    public class FakeConnection : NetworkConnectionToClient
    {
        public FakeConnection() : base(DummyNetworkConnection._idGenerator--)
        {
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