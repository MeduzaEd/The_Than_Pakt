using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class User_Interface : MonoBehaviour
{
    [SerializeField] Image SoundButton;
    [SerializeField] Scrollbar ScroolVolumeSound;
    [SerializeField] List<Sprite> SoundButtons=new List<Sprite>();
    
    public  bool IsMuteGame = false;
    public  float SongsVolume = 1f;
    private void Start()
    {
        _VolumeChange(1f);
        ScroolVolumeSound.onValueChanged.AddListener(_VolumeChange);
    }
    public void _VolumeChange(float _v)
    {
        IsMuteGame = false;
        SongsVolume = _v;
        if (SongsVolume>0f && SongsVolume<=0.25f)
        {
            SoundButton.sprite = SoundButtons[1];
        }
        else if(SongsVolume >0.25f && SongsVolume <=0.5f)
        {
            SoundButton.sprite = SoundButtons[2];
        }
        else if(SongsVolume > 0.5f && SongsVolume<=1f)
        {
            SoundButton.sprite = SoundButtons[3];
        }
        else if (SongsVolume <= 0f)
        {
            SoundButton.sprite = SoundButtons[0];
        }
    }
    public void _MuteUnMute()
    {
        IsMuteGame = !IsMuteGame;
        SoundButton.sprite= SoundButtons[0];
        if (!IsMuteGame) { _VolumeChange(SongsVolume); }
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
    public void _TheToExitGame()
    {
        GetComponent<Animator>().Play("ExitToGame");
    }
    private void _Exit_()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
