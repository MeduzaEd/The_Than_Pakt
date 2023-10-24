using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class _User_Script_ : NetworkBehaviour
{




    #region ____
    private GameObject _Camera;
    private Vector3 _CameraOffSet=new Vector3(0,0.25f,-0.2f);
    public GameObject Character;
    public string UserName="ayanayan132412";
    [Command]
    private void AddCharacter()
    {
        _Server_Scripts_ serverObject = FindObjectOfType<_Server_Scripts_>();
        serverObject.CreateCharacter(this.gameObject,SystemInfo.deviceUniqueIdentifier, UserName);
    }

    void Start()
    {
        if (!isLocalPlayer) return;
        _Camera = GameObject.FindObjectOfType<Camera>().gameObject;
        AddCharacter();
    }
    private void LateUpdate()
    {
        if (!isLocalPlayer||!Character|| !_Camera) return;
        _Camera.transform.position = Character.transform.GetChild(0).position + _CameraOffSet;
    }
    #endregion
}
