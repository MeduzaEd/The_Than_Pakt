using Unity.Netcode;
using UnityEngine;

public struct DiscoveryResponseData: INetworkSerializable
{
    public ushort Port;

    public string ServerName;

    public uint MaxConnections;

    public uint CurentConnections;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Port);
        serializer.SerializeValue(ref ServerName);
        serializer.SerializeValue(ref MaxConnections);
        serializer.SerializeValue(ref CurentConnections);
    }
}
