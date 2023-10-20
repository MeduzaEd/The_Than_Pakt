
using UnityEngine;
using Mirror;
using Mirror.Discovery;

public class _Custom_Discovery : NetworkDiscovery
{
    public event System.Action<ServerResponse> OnCustomServerFound;

    public new void OnServerFound(ServerResponse info)
    {
        OnCustomServerFound?.Invoke(info);
        Debug.Log("FOUND");
    }
}
