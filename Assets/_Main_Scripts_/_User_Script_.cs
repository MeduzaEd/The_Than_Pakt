using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class _User_Script_ : NetworkBehaviour
{
    private string SI_ID="None";
    private GameObject _Camera;
    private Vector3 _CameraOffSet=new Vector3(0,0.25f,-0.2f);
    public GameObject Character;
    [SyncVar]
    public string UserName="ayanayan132412";
    [Command]
    private void AddCharacter()
    {
        Debug.Log("ADDCharacter");
        _Server_Scripts_ serverObject = FindObjectOfType<_Server_Scripts_>();
        serverObject.CreateCharacter(SystemInfo.deviceUniqueIdentifier, UserName);
    }

    void Start()
    {

        Debug.Log($"isLocalPlayer:{isLocalPlayer},isClient:{isClient},isClientOnly:{isClientOnly},isOwned:{isOwned},isServerOnly:{isServerOnly},authority:{authority},netIdentity:{netIdentity.netId}");
        transform.SetParent(GameObject.FindGameObjectWithTag("_USERS_").transform);
        
        if (!isLocalPlayer) return;
        _Camera = GameObject.FindObjectOfType<Camera>().gameObject;
        SI_ID = SystemInfo.deviceUniqueIdentifier;
        this.name = SI_ID;
        AddCharacter();
    }
    private void LateUpdate()
    {
        if (!isLocalPlayer||!Character|| !_Camera) return;
        _Camera.transform.position = Character.transform.GetChild(0).position + _CameraOffSet;
    }
}
