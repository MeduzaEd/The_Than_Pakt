using Mirror.Discovery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _NMC : MonoBehaviour
{
    [SerializeField] _Custom_Discovery customDiscovery;

    void Start()
    {
        customDiscovery.OnCustomServerFound += OnServerFound;
        customDiscovery.StartDiscovery();
    }

    private void OnServerFound(ServerResponse info)
    {
        Debug.Log("Found server at IP: " + info.EndPoint.Address);
        Debug.Log("Found server on port: " + info.EndPoint.Port);
    }
    public void OnDiscovery(Mirror.Discovery.ServerResponse response)
    {
        Debug.Log("Found server on port: " + response.EndPoint.Port);
        Debug.Log("Found server at IP: " + response.EndPoint.Address);
    }
}
