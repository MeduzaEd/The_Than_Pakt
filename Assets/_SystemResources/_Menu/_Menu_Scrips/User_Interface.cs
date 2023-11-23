using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UNET;
using System.Linq;
using System;

public class User_Interface : MonoBehaviour
{
    [SerializeField] List<string> BadTexts;
    [SerializeField] Image SoundButton;
    [SerializeField] public Text ServerName;
    [SerializeField] public Scrollbar ScroolVolumeSound;
    [SerializeField] public Scrollbar ScroolVolumeMaxConnections;
    [SerializeField] List<Sprite> SoundButtons=new List<Sprite>();
    public _Cache_Save_System_ UserData;
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

        #endregion
    }
    public void _MaxUsersChange()
    {
        int Users = (int)(ScroolVolumeMaxConnections.value * 15);
        UserData.UserData.MaxUsersInHost =(Users+1);
        NetworkManager.Singleton.GetComponent<UNetTransport>().MaxConnections =UserData.UserData.MaxUsersInHost;
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

        Debug.Log("�������");
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

    public void _TheToExitGame()
    {
        GetComponent<Animator>().Play("ExitToGame");
    }
    private void _Exit_()
    {
        Debug.Log("Saving");
        UserData.SaveData();
        Debug.Log("Exiting");
        Application.Quit();
    }
}
