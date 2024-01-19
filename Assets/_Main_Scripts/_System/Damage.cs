using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Damage : NetworkBehaviour
{
    public ulong _OwnerID = ulong.MaxValue;
    public uint _DefaultDamage = 10;
    public List<uint> _Targeted = new();
     
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) { return; }
  
        if (collision.gameObject.layer == LayerMask.NameToLayer("PLayer"))
        {
            TheDamageServerRpc(_OwnerID,collision.transform.parent.GetComponent<NetworkObject>().OwnerClientId, _DefaultDamage);
        }
    }
    [ServerRpc]
    private void TheDamageServerRpc(ulong _MyId,ulong _Id,uint _Dmg)
    {
        if (!IsServer) { return; }
        Debug.Log("HasServer");
        Humanoid _Humanoid = NetworkManager.SpawnManager.GetPlayerNetworkObject(_Id).GetComponent<Humanoid>();
        _Humanoid.OnDamage(_MyId, _Dmg);
    }
}
