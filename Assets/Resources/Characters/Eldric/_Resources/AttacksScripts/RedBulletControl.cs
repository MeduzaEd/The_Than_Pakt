using Unity.Netcode;
using UnityEngine;

public class RedBulletControl : NetworkBehaviour
{
    private void Start()
    {
        Debug.Log("Despawn");
        if (!IsServer) { return; }
        StartCoroutine(WaitRealTime.WaitToRealTime.WaitRealTime(2f));
        transform.GetComponent<NetworkObject>().Despawn();
    }
}
