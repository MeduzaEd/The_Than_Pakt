using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Character_Type : NetworkBehaviour
{
    public string Type="none";
    private void Start()
    {
        if (!IsOwner) { return; }
        if(transform.GetComponentInChildren<Camera>())
        {
            transform.GetComponentInChildren<Camera>().enabled = true;
        }
    }
}
