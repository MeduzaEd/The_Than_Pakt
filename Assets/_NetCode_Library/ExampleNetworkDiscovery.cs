using System;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkManager))]
public class ExampleNetworkDiscovery : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
{
    [Serializable]
    public class ServerFoundEvent : UnityEvent<IPEndPoint, DiscoveryResponseData>
    {
    };

    NetworkManager m_NetworkManager;

    [SerializeField]
    [Tooltip("If true NetworkDiscovery will make the server visible and answer to client broadcasts as soon as netcode starts running as server.")]
    bool m_StartWithServer = true;

    public string ServerName = "EnterName";
    public uint MaxConnections = 10;
    public uint CurentConnections = 10;

    public ServerFoundEvent OnServerFound;

    private bool m_HasStartedWithServer = false;

    public void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
        m_NetworkManager.OnServerStarted += StartMainScene;
    }
    private void StartMainScene()
    {
        if (!m_NetworkManager.IsServer) { return; }
        m_NetworkManager.SceneManager.LoadScene("_Dungeon_Menu", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    public void Update()
    {
        if (m_StartWithServer && m_HasStartedWithServer == false && IsRunning == false)
        {
            if (m_NetworkManager.IsServer)
            {
                
                MaxConnections = (uint)m_NetworkManager.GetComponent<UNetTransport>().MaxConnections;
                StartServer();
                m_HasStartedWithServer = true;
            }
        }
        if(m_NetworkManager.IsServer&& m_HasStartedWithServer && CurentConnections != (uint)m_NetworkManager.ConnectedClients.Count)
        {
            CurentConnections = (uint)m_NetworkManager.ConnectedClients.Count;
            Debug.Log($"Servaer HAs Started Connections:{CurentConnections} & ");
        }
    }

    protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadCast, out DiscoveryResponseData response)
    {
        // Предполагая, что ConnectionData.Port возвращает int
        ushort port = ((ushort)((UNetTransport)m_NetworkManager.NetworkConfig.NetworkTransport).ServerListenPort);

        response = new DiscoveryResponseData()
        {
            ServerName = ServerName,
            Port = port,
            MaxConnections= MaxConnections,
            CurentConnections= CurentConnections,
        };
        return true;
    }

    protected override void ResponseReceived(IPEndPoint sender, DiscoveryResponseData response)
    {
        OnServerFound.Invoke(sender, response);
    }
    
}
