using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConnectionToServer : MonoBehaviour
{
    public string _Adress="127.0.0.1";
    public ushort _Port = 7777;
    public NetworkManager m_NetworkManager;
    private Text _pingtext;
    public void connectclient()
    {
        UNetTransport transport = (UNetTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
        transport.ConnectAddress = _Adress;
        transport.ConnectPort = _Port;
        m_NetworkManager.StartClient();
    }
    private void Start()
    {
        m_NetworkManager = GameObject.FindObjectOfType<NetworkManager>();
        _pingtext = transform.GetChild(0).GetChild(2).GetComponent<Text>();
        StartCoroutine(serverchecker());
        _pingtext.text = $"Ping:{999}+ ms";
    }
    IEnumerator serverchecker()
    {
        yield return null;
        do
        {
            StartCoroutine(PingServer());
            yield return new WaitForSecondsRealtime(1f);
            yield return null;
        } while (true);
    }
    IEnumerator PingServer()
    {
        Ping pn = new Ping(_Adress);
        while (!pn.isDone)
        {
            yield return null;
        }
        _pingtext.text = $"Ping:{pn.time} ms";
        yield return null;

    }


}
