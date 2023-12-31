using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UNET;
using System.Linq;
using System;
using System.Net;
using UnityEngine.Audio;

public class User_Interface : MonoBehaviour
{
    [SerializeField] List<string> BadTexts;
    public static List<string> GlobalBadTexts;
    [SerializeField] Text IpAdress;
    [SerializeField] Transform ServersUIContent;
    [SerializeField] GameObject ServerUIprefab;
    [SerializeField] Image SoundButton;
    [SerializeField] List<Sprite> SoundButtons=new();
    [SerializeField] Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new();
    public Text ServerName;
    public Slider ScroolVolumeSound;
    public RectTransform _Content;
    public Scrollbar ScroolVolumeMaxConnections;
    public AudioMixer _mixer;
    public Cache_Save_System_ UserData;
    private NetworkManager _NetworkManager;
    private ExampleNetworkDiscovery m_Discovery;
   
    public IEnumerator AutoSearchServers()
    {
        yield return new WaitForSecondsRealtime(3f);
        do
        {

           // Searchservers();
            yield return new WaitForSecondsRealtime(3f);
            yield return null;
        }
        while (true);
        //yield return null;
    }
    public void OnServerFound(IPEndPoint sender, DiscoveryResponseData response)
    {
        discoveredServers[sender.Address] = response;
        ReloadServersUI();
        Debug.Log("serverfownded");
    }
    public void Searchservers()
    {
        Debug.Log(discoveredServers.Values.Count);
        if (!m_Discovery.IsRunning)
        {
            m_Discovery.StartClient(); 
        }
    
        m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
       
        //Debug.Log(discoveredServers.Values.Count);
        //Debug.Log(discoveredServers.Keys);
    }
    public void ReloadServersUI()
    {
        try
        {
            if (ServersUIContent.childCount > 0)
            {
                ServersUIContent.DetachChildren();
            }
        }
        catch { }
        if (discoveredServers.Values.Count > 0)
        {
            foreach (var server in discoveredServers)
            {
                GameObject NewServerUI = Instantiate(ServerUIprefab, ServersUIContent);
                NewServerUI.GetComponent<NetworkConnectionToServer>()._Adress = server.Key.ToString();
                NewServerUI.GetComponent<NetworkConnectionToServer>()._Port = server.Value.Port;
                NewServerUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = server.Value.ServerName;
                NewServerUI.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = $"Users:{server.Value.CurentConnections}/{server.Value.MaxConnections}";
                NewServerUI.transform.SetParent(ServersUIContent, false);
            }
            discoveredServers.Clear();
            Debug.Log("cleared");
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
            IpAdress.text = Checktoip(IpAdress.text);
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
    private string Checktoip(string _adress)
    {
        return _adress.Trim().ToLower();
    }
    public void Start()
    {
      
        #region Load Local Data's
        UserData = GameObject.FindObjectOfType<Cache_Save_System_>();
        #endregion

        #region UI's Load
        ImageChange();
        #endregion
        #region Actions

        ScroolVolumeSound.onValueChanged.AddListener(VolumeChange);
        GlobalBadTexts = BadTexts;
        #endregion
        _NetworkManager = GameObject.FindObjectOfType<NetworkManager>();
        m_Discovery = GameObject.FindObjectOfType<ExampleNetworkDiscovery>();

        StartCoroutine(AutoSearchServers());
 
    }
    public void MaxUsersChange()
    {
        int Users = (int)(ScroolVolumeMaxConnections.value * 15);
        UserData.UserData.MaxUsersInHost =(Users+1);

        GameObject.FindObjectOfType<UNetTransport>().MaxConnections =UserData.UserData.MaxUsersInHost;
        TextChangeInMaxUsers();
    }
    public void TextChangeInMaxUsers()
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
    public void ServerNameChange(string _text)
    {
        _text = ServerNameFormat(_text);
        ServerName.text = _text;
        ServerName.transform.parent.GetComponent<InputField>().text = _text;
        UserData.UserData.MyServerName = _text;
        GameObject.FindObjectOfType<ExampleNetworkDiscovery>().ServerName = UserData.UserData.MyServerName;
    }
    public void ImageChange()
    {
        Debug.Log(UserData.UserData.SoundsVolume);
        UserData.UserData.SoundsVolume = ScroolVolumeSound.value;
        if (UserData.UserData.SoundsVolume > -80f && UserData.UserData.SoundsVolume <= -40f)
        {
            SoundButton.sprite = SoundButtons[1];
        }
        else if (UserData.UserData.SoundsVolume > -40f && UserData.UserData.SoundsVolume <= 0)
        {
            SoundButton.sprite = SoundButtons[2];
        }
        else if (UserData.UserData.SoundsVolume > 0f && UserData.UserData.SoundsVolume <= 20f)
        {
            SoundButton.sprite = SoundButtons[3];
        }
        else if (UserData.UserData.SoundsVolume <= -40f)
        {
            SoundButton.sprite = SoundButtons[0];
        }
        
       // _mixer.SetFloat("Volume", ScroolVolumeSound.value>0? ScroolVolumeSound.value*20f: (ScroolVolumeSound.value ==0 ?0f: ScroolVolumeSound.value*80f));

        if (UserData.UserData.SoundsIsMute) { SoundButton.sprite = SoundButtons[0]; }
    }
    public void VolumeChange(float _v)
    {

        Debug.Log("�������");
        UserData.UserData.SoundsIsMute = _v<0;
        UserData.UserData.SoundsVolume = _v;
        _mixer.SetFloat("Volume",_v);
        ImageChange();
       
    }
    public void MuteUnMute()
    {
        UserData.UserData.SoundsIsMute = !UserData.UserData.SoundsIsMute;
        SoundButton.sprite= SoundButtons[0];
        if (!UserData.UserData.SoundsIsMute) {VolumeChange(UserData.UserData.SoundsVolume); }
        else { VolumeChange(-80f); }
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
    public void Scroll_Content(Vector2 _)
    {
        int _y =((_Content.transform.childCount * 200) - 450)<50?50: ((_Content.transform.childCount * 200) - 450);
        _Content.localPosition = new Vector3(0, _Content.localPosition.y < 0 ? 0 : _Content.localPosition.y);//min
        _Content.localPosition = new Vector3(0,  _Content.localPosition.y > _y ? _y: _Content.localPosition.y);//max
       
        //Debug.Log(_Content.localPosition.y);
    }
    public void TheToExitGame()
    {
        GetComponent<Animator>().Play("ExitToGame");
    }
    public void HostGameAnimation()
    {
        GetComponent<Animator>().Play("StartHostAnimation");
    }
    public void HostGame()
    {
        _NetworkManager.StartHost();
    }
    private void OnApplicationQuit()
    {
        Exit_();
    }
    private void Exit_()
    {
        Debug.Log("Saving");
        UserData.SaveData();
        Debug.Log("Exiting");
        Application.Quit();
    }
}
