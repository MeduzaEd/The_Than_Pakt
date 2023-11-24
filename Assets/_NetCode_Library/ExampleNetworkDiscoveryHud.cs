using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using System.Collections;

[RequireComponent(typeof(ExampleNetworkDiscovery))]
[RequireComponent(typeof(NetworkManager))]
public class ExampleNetworkDiscoveryHud : MonoBehaviour
{
    [SerializeField, HideInInspector]
    ExampleNetworkDiscovery m_Discovery;

     NetworkManager m_NetworkManager;

    Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new Dictionary<IPAddress, DiscoveryResponseData>();

    public Vector2 DrawOffset = new Vector2(10, 210);

    void Awake()
    {
        m_Discovery = GetComponent<ExampleNetworkDiscovery>();
        m_NetworkManager = GetComponent<NetworkManager>();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (m_Discovery == null) // This will only happen once because m_Discovery is a serialize field
        {
            m_Discovery = GetComponent<ExampleNetworkDiscovery>();
            m_Discovery.OnServerFound.AddListener(OnServerFound);
            UnityEditor.Undo.RecordObjects(new Object[] { this, m_Discovery }, "Set NetworkDiscovery");
        }
    }
#endif

    public void OnServerFound(IPEndPoint sender, DiscoveryResponseData response)
    {
        discoveredServers[sender.Address] = response;
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(DrawOffset, new Vector2(200, 600)));

        if (m_NetworkManager.IsServer || m_NetworkManager.IsClient)
        {
            if (m_NetworkManager.IsServer)
            {
                ServerControlsGUI();
            }
        }
        else
        {
            ClientSearchGUI();
        }

        GUILayout.EndArea();
    }
    IEnumerator PingCheck(string ip)
    {
        Ping pn = new Ping(ip);
        while (!pn.isDone)
        {
            yield return null;
        }
        
        yield return pn.time.ToString();
    }
    void ClientSearchGUI()
    {
        if (m_Discovery.IsRunning)
        {
            if (GUILayout.Button("Stop Client Discovery"))
            {
                m_Discovery.StopDiscovery();
                discoveredServers.Clear();
            }

            if (GUILayout.Button("Refresh List"))
            {
                discoveredServers.Clear();
                m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
            }

            GUILayout.Space(40);

            foreach (var discoveredServer in discoveredServers)
            {
                
                if (GUILayout.Button($"{discoveredServer.Value.ServerName}[{discoveredServer.Key.ToString()}] Ping:{  StartCoroutine(PingCheck(discoveredServer.Key.ToString())) }"))
                {
                    UNetTransport transport = (UNetTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
                    transport.ConnectAddress = discoveredServer.Key.ToString();
                    transport.ConnectPort = (ushort)discoveredServer.Value.Port;
                    m_NetworkManager.StartClient();
                }
            }
        }
        else
        {
            if (GUILayout.Button("Discover Servers"))
            {
                m_Discovery.StartClient();
                m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
            }
        }
    }

    void ServerControlsGUI()
    {
        if (m_Discovery.IsRunning)
        {
            if (GUILayout.Button("Stop Server Discovery"))
            {
                m_Discovery.StopDiscovery();
            }
        }
        else
        {
            if (GUILayout.Button("Start Server Discovery"))
            {
                m_Discovery.StartServer();
            }
        }
    }
}
