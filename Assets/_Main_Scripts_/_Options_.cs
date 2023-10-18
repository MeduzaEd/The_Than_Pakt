using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Net.NetworkInformation;

using Ping = UnityEngine.Ping;
using System.Net;
using System.Net.Sockets;

public class _Options_ : MonoBehaviour
{
    NetworkManager NM;
    [Header("Host")]
    [SerializeField]
    private Slider CL;
    [SerializeField]
    private Slider CD;
    [SerializeField]
    private Slider PT;
    [Header("Connection By Ip")]
    [SerializeField]
    private Text _Adress;
    [SerializeField]
    private Text _Port;
    [Header("Connections")]
    [SerializeField]
    private GameObject _Servers;
    public void Change()
    {
        try
        {
            NM.GetComponent<kcp2k.KcpTransport>().port = (ushort)PT.value;
        }
        catch{}
        try
        {
            NM.disconnectInactiveTimeout = (int)CD.value;
        }
        catch { }
        try
        {
            NM.maxConnections= (int)CL.value;
        }
        catch { }
        try
        {
            CL.transform.parent.GetComponent<Text>().text = $"Conn Limit:{CL.value} Users";
        }
        catch { }
        try
        {
            CD.transform.parent.GetComponent<Text>().text = $"Disconn Inactives:{CD.value} Min";
        }
        catch { }
        try
        {
            PT.transform.parent.GetComponent<Text>().text = $"Port:{PT.value}";
        }
        catch { }
    }
    private void Start()
    {
        NM = NetworkManager.singleton;
        Change();
        ScanLocalNetwork();
    }
    private void ScanLocalNetwork()
    {
        string baseIP = "192.168.1.";
        for (int i = 1; i <= 255; i++)
        {
            string ip = baseIP + i.ToString();
            _Ping(ip);
            Debug.Log("Устройство с IP-адресом " + ip + " в проверке.");
        }
    }
    private IEnumerator _Ping(string i)
    {
        Debug.Log("Устройство с IP-адресом " + i + " активно в проверке.");
        Ping ping = new Ping(i);
    
        if (ping.isDone == true)
        {
            Debug.Log("Устройство с IP-адресом " + i + " активно.");
        }
        else
        {
            Debug.Log("Устройство с IP-адресом " + i + "не активно.");
        }
        Debug.Log("Устройство с IP-адресом " + i + " проверенно.");
        yield return null;
    }
    public void ConnectionToIP()
    {
        try
        {
            NM.networkAddress = _Adress.text;
            NM.GetComponent<kcp2k.KcpTransport>().port =(ushort) int.Parse(_Port.text);
            NM.StartClient();
    
            Debug.Log("Connected");
        }
        catch { Debug.Log("Error"); }
    }


}
