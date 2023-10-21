
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;
using System.Collections.Generic;

public class _Options_ : MonoBehaviour
{
    [SerializeField]
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
    [SerializeField]
    private GameObject _ServerUI;
    [SerializeField]
    private Mirror.Discovery.NetworkDiscovery DY;
    public readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
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
        discoveredServers.Clear();
        DY.StartDiscovery();
        // DY =GetComponent< Mirror.Discovery.NetworkDiscovery >();
        Change();

    }
    public void StartHostDY()
    {
        discoveredServers.Clear();
        NM.StartHost();
        DY.AdvertiseServer();
    }

    public void OnConnectedFromDiscovery()
    {
        discoveredServers.Clear();
        DY.StopDiscovery();
    }
    public void ConnectionToIP()
    {
        try
        {
            discoveredServers.Clear();
            NM.networkAddress = _Adress.text;
            NM.GetComponent<kcp2k.KcpTransport>().port =(ushort) int.Parse(_Port.text);
            NM.StartClient();
            Debug.Log("Connected");
        }
        catch { Debug.Log("Error"); }
    }
 
    public void FoundedServers(ServerResponse data)
    {
        string Adress = data.uri.DnsSafeHost;
        string Port = data.uri.Port.ToString();
        GameObject _ServerUI_ = Instantiate(_ServerUI, _Servers.transform,false);
        _Server_Connection_ SC = _ServerUI_.GetComponent<_Server_Connection_>();
        SC.Port = Port;
        SC.Adress = Adress;
        SC.NM = NM;
        SC.options = this;
        _ServerUI.transform.GetComponent<RectTransform>().sizeDelta=new Vector2(850,100);
        //_ServerUI.transform.localScale = new Vector3(1f, 1f, 1f);

        Debug.Log(Adress +":"+ Port);
    }
    public void Referesh()
    {
        discoveredServers.Clear();
        DY.StartDiscovery();

    }
    public void Roll()
    {
        RectTransform rect = _Servers.GetComponent<RectTransform>();
        float _y = _Servers.transform.childCount>4?(_Servers.transform.childCount * 125f)-600f:25;
        //Debug.Log(rect.localPosition);
       // Debug.Log(rect.position);
        //Debug.Log(_y);
        if (rect.localPosition.y < -5f)
        {
            rect.localPosition= new Vector3(425,-5,0);
            Debug.Log("Y>5");
            //   rect.position = new Vector3(0,5,0);
        }
        else if (rect.localPosition.y > _y)
        {
            rect.localPosition = new Vector3(425, _y, 0);
            Debug.Log("Y>_y");
        }
    }
}
