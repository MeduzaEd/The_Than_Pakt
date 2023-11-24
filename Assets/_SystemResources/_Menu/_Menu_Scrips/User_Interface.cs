using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UNET;
using System.Linq;
using System;
using System.Net;

public class User_Interface : MonoBehaviour
{
    [SerializeField] List<string> BadTexts;
    public static List<string> GlobalBadTexts;
    [SerializeField] Text IpAdress;
    [SerializeField] Transform ServersUIContent;
    [SerializeField] GameObject ServerUIprefab;
    [SerializeField] Image SoundButton;
    [SerializeField] public Text ServerName;
    [SerializeField] public Scrollbar ScroolVolumeSound;
    [SerializeField] public RectTransform _Content;
    [SerializeField] public Scrollbar ScroolVolumeMaxConnections;
    [SerializeField] List<Sprite> SoundButtons=new List<Sprite>();
    [SerializeField] Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new Dictionary<IPAddress, DiscoveryResponseData>();

    public _Cache_Save_System_ UserData;
    private NetworkManager _NetworkManager;
    private ExampleNetworkDiscovery m_Discovery;
    public void OnServerFound(IPEndPoint sender, DiscoveryResponseData response)
    {
        discoveredServers[sender.Address] = response;
       
    }
    public void searchservers()
    {
        Debug.Log(discoveredServers.Values.Count);
        if (!m_Discovery.IsRunning)
        {
            m_Discovery.StartClient(); 
        }
        m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
        ReloadServersUI();
        Debug.Log(discoveredServers.Values.Count);
        Debug.Log(discoveredServers.Keys);
    }
    public void ReloadServersUI()
    {
        if (ServersUIContent.childCount > 0)
        {
            ServersUIContent.DetachChildren();
        }
        if (discoveredServers.Values.Count > 0)
        {
            foreach (var server in discoveredServers)
            {
                GameObject NewServerUI = Instantiate(ServerUIprefab, ServersUIContent);
                NewServerUI.GetComponent<NetworkConnectionToServer>()._Adress = server.Key.ToString();
                NewServerUI.GetComponent<NetworkConnectionToServer>()._Port = server.Value.Port;
                NewServerUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = server.Value.ServerName;
                NewServerUI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = $"Users:{server.Value.CurentConnections}/{server.Value.MaxConnections}";
            }
            discoveredServers.Clear();
        }
        //m_Discovery.StopDiscovery();
    }
    public void ConnectFromIP()
    {
        try
        {
            if (_NetworkManager.IsConnectedClient)
            {
                _NetworkManager.DisconnectClient(_NetworkManager.LocalClientId);
            }
            IpAdress.text = checktoip(IpAdress.text);
            UNetTransport transport = (UNetTransport)_NetworkManager.NetworkConfig.NetworkTransport;
            transport.ConnectAddress = IpAdress.text;
            transport.ConnectPort = 7777;
            _NetworkManager.StartClient();
        }
        catch (Exception ex) 
        { 
            Debug.Log(ex); 
        }
    }
    private string checktoip(string _adress)
    {
        return _adress.Trim().ToLower();
    }
    private void Start()
    {
        #region Load Local Data's
        UserData = GameObject.FindObjectOfType<_Cache_Save_System_>();
        #endregion

        #region UI's Load
        _ImageChange();
        #endregion
        #region Actions
        ScroolVolumeSound.onValueChanged.AddListener(_VolumeChange);
        GlobalBadTexts = BadTexts;
        #endregion
        _NetworkManager = GameObject.FindObjectOfType<NetworkManager>();
        m_Discovery = GameObject.FindObjectOfType<ExampleNetworkDiscovery>();
    }
    public void _MaxUsersChange()
    {
        int Users = (int)(ScroolVolumeMaxConnections.value * 15);
        UserData.UserData.MaxUsersInHost =(Users+1);

        GameObject.FindObjectOfType<UNetTransport>().MaxConnections =UserData.UserData.MaxUsersInHost;
        _TextChangeInMaxUsers();
    }
    public void _TextChangeInMaxUsers()
    {
        ScroolVolumeMaxConnections.transform.parent.GetChild(0).GetComponent<Text>().text = $"max connections:{UserData.UserData.MaxUsersInHost}";
    }
    private string ServerNameFormat(string _text)
    {
        string defoldservername = "Default-Server";
        try
        {
            if (_text.Replace(" ", string.Empty).Length != _text.Length)
            {
                Debug.Log("Server name has not use spaces");
                _text = _text.Replace(" ", string.Empty);
            }

            if (_text.Length > 32 || _text.Length < 6)
            {
                Debug.Log("Server name has not upper 32 & lower 6 symb");
                return defoldservername;
            }
            foreach (string _Bad_text in BadTexts)
            {
                if (_text.ToLower().Contains(_Bad_text.ToLower()))
                {
                    Debug.Log("Please dont use Bad texts");
                    return defoldservername;
                }
            }
        }
        catch(Exception ex) 
        { 
            Debug.Log(ex); 
            return defoldservername; 
        }

        return _text;
    }
    public void _ServerNameChange(string _text)
    {
        _text = ServerNameFormat(_text);
        ServerName.text = _text;
        ServerName.transform.parent.GetComponent<InputField>().text = _text;
        UserData.UserData.MyServerName = _text;
        GameObject.FindObjectOfType<ExampleNetworkDiscovery>().ServerName = UserData.UserData.MyServerName;
    }
    public void _ImageChange()
    {
        if (UserData.UserData.SoundsVolume > 0f && UserData.UserData.SoundsVolume <= 0.25f)
        {
            SoundButton.sprite = SoundButtons[1];
        }
        else if (UserData.UserData.SoundsVolume > 0.25f && UserData.UserData.SoundsVolume <= 0.5f)
        {
            SoundButton.sprite = SoundButtons[2];
        }
        else if (UserData.UserData.SoundsVolume > 0.5f && UserData.UserData.SoundsVolume <= 1f)
        {
            SoundButton.sprite = SoundButtons[3];
        }
        else if (UserData.UserData.SoundsVolume <= 0f)
        {
            SoundButton.sprite = SoundButtons[0];
        }
        ScroolVolumeSound.value = UserData.UserData.SoundsVolume;
        if (UserData.UserData.SoundsIsMute) { SoundButton.sprite = SoundButtons[0]; }
    }
    public void _VolumeChange(float _v)
    {

        Debug.Log("Ñðôòïóâ");
        UserData.UserData.SoundsIsMute = _v>0?false:true;
        UserData.UserData.SoundsVolume = _v;
        
        _ImageChange();
       
    }
    public void _MuteUnMute()
    {
        UserData.UserData.SoundsIsMute = !UserData.UserData.SoundsIsMute;
        SoundButton.sprite= SoundButtons[0];
        if (!UserData.UserData.SoundsIsMute) { _VolumeChange(UserData.UserData.SoundsVolume); }
    }
    public void AnimationSettingsChange(bool _To)
    {
        if(_To)
        {
            GetComponent<Animator>().Play("Open_Settings");
        }
        else
        {
            GetComponent<Animator>().Play("Close_Settings");
        }
    }
    public void AnimationPlayMenuChange(bool _To)
    {
        if (_To)
        {
            GetComponent<Animator>().Play("Open_Play_Menu");
        }
        else
        {
            GetComponent<Animator>().Play("Close_Play_Menu");
        }
    }
    public void AnimationExitGameChange(bool _To)
    {
        if (_To)
        {
            GetComponent<Animator>().Play("Open_Exit_Menu");
        }
        else
        {
            GetComponent<Animator>().Play("Close_Exit_Menu");
        }
    }

    public void AnimationMenuHostChange(bool _To)
    {
        if (_To)
        {
            GetComponent<Animator>().Play("Open_Host");
        }
        else
        {
            GetComponent<Animator>().Play("Close_Host");
        }
    }
    public void AnimationMenuConnectChange(bool _To)
    {
        if (_To)
        {
            GetComponent<Animator>().Play("Open_Connect_Menu");
        }
        else
        {
            GetComponent<Animator>().Play("Close_Connect_Menu");
        }
    }
    public void _Scroll_Content(Vector2 _)
    {
        int _y =((_Content.transform.childCount * 200) - 450)<50?50: ((_Content.transform.childCount * 200) - 450);
        _Content.localPosition = new Vector3(0, _Content.localPosition.y < 0 ? 0 : _Content.localPosition.y);//min
        _Content.localPosition = new Vector3(0,  _Content.localPosition.y > _y ? _y: _Content.localPosition.y);//max
       
        Debug.Log(_Content.localPosition.y);
    }
    public void _TheToExitGame()
    {
        GetComponent<Animator>().Play("ExitToGame");
    }
    public void _HostGameAnimation()
    {
        GetComponent<Animator>().Play("StartHostAnimation");
    }
    public void _HostGame()
    {
        _NetworkManager.StartHost();
    }

    private void _Exit_()
    {
        Debug.Log("Saving");
        UserData.SaveData();
        Debug.Log("Exiting");
        Application.Quit();
    }
}
