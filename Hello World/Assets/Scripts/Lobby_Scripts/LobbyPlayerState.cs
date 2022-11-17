using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

// code from tutorial - https://www.youtube.com/watch?v=sBR0oJJjx6Q and https://www.youtube.com/watch?v=PnQutPyMZhI

public struct LobbyPlayerState : INetworkSerializable, IEquatable<LobbyPlayerState>
{
    public ulong ClientID;
    public FixedString32Bytes PlayerName;
    public int HelmetNum;
    public bool IsReady;

    public LobbyPlayerState(ulong clientID, string playerName, int helmetNum, bool isReady)
    {
        ClientID = clientID;
        PlayerName = playerName;
        HelmetNum = helmetNum;
        IsReady = isReady;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientID);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref HelmetNum);
        serializer.SerializeValue(ref IsReady);
    }

    public bool Equals(LobbyPlayerState other)
    {
        return ClientID == other.ClientID &&
                            PlayerName.Equals(other.PlayerName) &&
                            HelmetNum.Equals(other.HelmetNum) &&
                            IsReady == other.IsReady;
    }
}
