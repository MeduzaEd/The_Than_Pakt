
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery;
using System.Collections.Generic;
using System;

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
    [SerializeField]
    private Text QualityUpdate;
    [SerializeField]
    private Text FPSUpdate;
    //public readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    public void QualitySet(int qualitylevel)
    {
        QualitySettings.SetQualityLevel(qualitylevel);
    }
    public void ChangeVsync(bool val)
    {
        QualitySettings.vSyncCount = val == true ? 1 : 0;
    }
    public void FPSSet(int levele)
    {
        uint FPS=30;
        switch (levele)
        {
            case 0:
                FPS = 30;
                break;
            case 1:
                FPS = 60;
                break;
            case 2:
                FPS = 120;
                break;
            case 3:
                FPS = 265;
                break;
            default:
                FPS = 30;
                break;
        }
        Application.targetFrameRate = (int)FPS;
    }
    private void Update()
    {
        string q = "none";
        switch(QualitySettings.GetQualityLevel())
        {
            case 0:
                q = "Low";
                
                break;
            case 1:
                q = "Normal";
                break;
            case 2:
                q = "Hight";
                break;
            case 3:
                q = "Ultra";
                FPSSet(3);
                break;
            default:
                q = "Error";
                break;
        }
        QualityUpdate.text = $"Current Quality:{q}";
        switch (Application.targetFrameRate)
        {
            case 30:
                q = "Low";
                break;
            case 60:
                q = "Normal";
                break;
            case 120:
                q = "Hight";
                break;
            case 265:
                q = "Ultra";
                break;
            default:
                q = "Error";
                break;
        }
        FPSUpdate.text = $"Current FPS Limit:{q}";

    }
    private List<ServerResponse> serverslist=new List<ServerResponse>();
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
     //   discoveredServers.Clear();
        DY.StartDiscovery();
        // DY =GetComponent< Mirror.Discovery.NetworkDiscovery >();
        Change();

    }
    public void StartHostDY()
    {
      //  discoveredServers.Clear();
        NM.StartHost();
        DY.AdvertiseServer();
    }

    public void OnConnectedFromDiscovery()
    {
     //   discoveredServers.Clear();
        DY.StopDiscovery();
    }
    public void ConnectionToIP()
    {
        try
        {
          //  discoveredServers.Clear();
            NM.networkAddress = _Adress.text;
            NM.GetComponent<kcp2k.KcpTransport>().port =(ushort) int.Parse(_Port.text);
            NM.StartClient();
            Debug.Log("Connected");
        }
        catch { Debug.Log("Error"); }
    }
 
    public void FoundedServers(ServerResponse data)
    {
        try
        {
            if (!serverslist.Contains(data)) // Проверяем, не существует ли элемента в списке
            {
                serverslist.Add(data); // Если не существует, добавляем его
            }
        }
        catch {}
    }
    public void Referesh()
    {
        try
        {
            if (_Servers.transform.childCount > 0)
            {
                foreach (Transform server_ in _Servers.transform)
                {
                    Destroy(server_.gameObject);
                }

            }
            if (serverslist.Count > 0)
            {
                foreach (ServerResponse data in serverslist)
                {
                    string Adress = data.uri.DnsSafeHost;
                    string Port = data.uri.Port.ToString();
                    GameObject _ServerUI_ = Instantiate(_ServerUI, _Servers.transform, false);
                    _Server_Connection_ SC = _ServerUI_.GetComponent<_Server_Connection_>();
                    SC.Port = Port;
                    SC.Adress = Adress;
                    SC.NM = NM;
                    SC.options = this;
                    _ServerUI.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(850, 100);
                }
            }
        }
        catch(Exception ex) { Debug.Log(ex); }
        //  discoveredServers.Clear();
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
        }
        else if (rect.localPosition.y > _y)
        {
            rect.localPosition = new Vector3(425, _y, 0);
        }
    }
}
