using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System.Collections.Generic;

public class NetworkManagerExample : NetworkManager
{
    [SerializeField] _Custom_Discovery customDiscovery;

    public override void OnStartServer()
    {
        customDiscovery.AdvertiseServer();
    }
}
