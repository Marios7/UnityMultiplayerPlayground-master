using Unity.Collections;
using Unity.Netcode;

/// <summary>
/// To Support string, because NetworkVariable does not support them.
/// </summary>
public struct NetworkString : INetworkSerializable
{
    private FixedString4096Bytes info;
 
    //It passes the info to the NetworkBehavouir
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref info);
    }

    public override string ToString()
    {
        return info.ToString();
    }

    public static implicit operator string(NetworkString s) => s.ToString();
    public static implicit operator NetworkString(string s) => new NetworkString() { info = new FixedString4096Bytes(s) };
}
