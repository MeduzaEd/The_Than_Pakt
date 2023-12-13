using Unity.Netcode;
using UnityEngine;

public class RedBulletControl : NetworkBehaviour
{
    private void Start()
    {
        if (!IsServer) { return; }
        _Destroytime = 3f + Time.time;
        
    }
    public float _Destroytime;
    private void Update()
    {
        Debug.Log("Despawn");
        if ((!IsServer)||(Time.time<_Destroytime)) { return; }
        transform.GetComponent<NetworkObject>().Despawn();
    }
    [ServerRpc]
    public void ImpulseServerRpc()
    {
        Debug.Log("Impulse");
        if (!IsServer) { return; }
        GetComponent<Rigidbody>().AddForce(transform.forward * 777f, ForceMode.Force);
    }
}
