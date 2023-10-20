using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class _Server_Connection_ : MonoBehaviour
{
    public string Adress;
    public string Port;
    public NetworkManager NM;
    public _Options_ options;
    public void ConnectToServer()
    {
        options.discoveredServers.Clear();
        NM.networkAddress = Adress;
        NM.GetComponent<kcp2k.KcpTransport>().port = (ushort)int.Parse(Port);
        NM.StartClient();
    }
}
