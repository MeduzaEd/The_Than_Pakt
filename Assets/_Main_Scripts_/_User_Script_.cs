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
    [Command]
    private void AddCharacter()
    {
        _Server_Scripts_ serverObject = FindObjectOfType<_Server_Scripts_>();
        serverObject.CreateCharacter(SystemInfo.deviceUniqueIdentifier,"Ayan");
    }

    void Start()
    {
        Debug.Log($"isLocalPlayer:{isLocalPlayer},isClient:{isClient},isClientOnly:{isClientOnly},isOwned:{isOwned},isServerOnly:{isServerOnly},authority:{authority},netIdentity:{netIdentity}");
        if (!isLocalPlayer) return;
        transform.SetParent(GameObject.FindGameObjectWithTag("_USERS_").transform);
        _Camera = GameObject.FindObjectOfType<Camera>().gameObject;
        SI_ID = SystemInfo.deviceUniqueIdentifier;
        this.name = SI_ID;
        AddCharacter();
    }
    private void LateUpdate()
    {
        if (!isLocalPlayer||!Character) return;
        _Camera.transform.position = Character.transform.GetChild(0).position + _CameraOffSet;
    }
}
