using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class _Server_Connection_ : MonoBehaviour
{
    public string Adress;
    public string Port;
    public NetworkManager NM;
    public _Options_ options;
    void Start()
    {
        transform.GetChild(0).GetComponent<Text>().text = $"Server Adress:{Adress};";
        transform.GetChild(1).GetComponent<Text>().text = $"Server Port:{Port};";
    }
    public void ConnectToServer()
    {
       // options.discoveredServers.Clear();
        NM.networkAddress = Adress;
        NM.GetComponent<kcp2k.KcpTransport>().port = (ushort)int.Parse(Port);
        NM.StartClient();
    }

}
