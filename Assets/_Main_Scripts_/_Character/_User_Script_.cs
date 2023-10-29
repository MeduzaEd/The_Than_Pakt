using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class _User_Script_ : NetworkBehaviour
{
    #region _Variables_
    private GameObject _Camera;
    private Vector3 _CameraOffSet=new Vector3(0,0.25f,-0.2f);

    _Server_Scripts_ _ServerObject;
    #endregion

    //////////////////////////////////////////////////////////////////////////////

    #region _Voids_
    public override void OnStartLocalPlayer()
    {
        if ( !isLocalPlayer|| !isClient) return;
        _Camera = GameObject.FindObjectOfType<Camera>().gameObject;
        _ServerObject = FindObjectOfType<_Server_Scripts_>();
        Debug.Log("Loaded");
    }

    #endregion

}
