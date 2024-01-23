using Unity.Netcode;
using UnityEngine;

public class RedBulletControl : NetworkBehaviour
{
    private Animator _Animator;
   // public ulong _AttackOwner=ulong.MaxValue;
    private void Start()
    {
        if (!IsOwner) { return; }
        _Destroytime = 1.75f + Time.time;
        _Animator=GetComponent<Animator>();
    }
    public float _Destroytime;
    private void Update()
    {
        //Debug.Log("Despawn");
        if ((!IsOwner) ||(Time.time<_Destroytime)) { return; }
        _Animator = GetComponent<Animator>();
        Debug.Log("Despawn");
        _Animator.SetBool("Destroy", true);
    }
    public void Despawne()
    {
        if (!IsServer) { return; }
        transform.GetComponent<NetworkObject>().Despawn();
    }
    
    public void ImpulseServer()
    {
       
        if (!IsOwner) { Debug.Log("returned");  return; }
        Debug.Log("Impulse");
        ImpulseServerRpc();
    }
    [ServerRpc]
    public void ImpulseServerRpc()
    {
        if (!IsServer) { return; }
        GetComponent<Rigidbody>().AddForce(transform.forward * 2800f, ForceMode.Force);
    }
    private void OnTriggerEnter(Collider _obj)
    {
        if (!IsServer) { return; }
        if(_obj.CompareTag("_Toucnhable_"))
        {
            Debug.Log($"Touched here as to :{_obj.name}");
            _Animator = GetComponent<Animator>();
            _Animator.SetBool("Destroy", true);

        }
        //OnDamange();
        //OnDamange(_obj.transform.parent.GetComponent<NetworkObject>().NetworkObjectId, _AttackOwner);
    }
    //private void OnDamange()
    //{
    //    if (!IsServer) { return; }
    //    //Humanoid _humanoid = NetworkManager.SpawnManager.GetPlayerNetworkObject(ownerid).GetComponent<Humanoid>();
    //   // Humanoid _enemyhumanoid = NetworkManager.SpawnManager.GetPlayerNetworkObject(_id).GetComponent<Humanoid>();
    //   // _enemyhumanoid.Health.Value -= (uint)((_humanoid.MagicPower.Value * 0.3f) + (_humanoid.PhysicPower.Value * 0.45f));
    //   // _enemyhumanoid.PlayerKilled.Value = ownerid;
    //}
}
