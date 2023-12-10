using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Eldric_Control : NetworkBehaviour
{
    private Transform _Camera;
    private void Start()
    {
        if (!IsOwner) return;
        _Camera = transform.parent.GetChild(0).GetChild(0).transform;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if(Input.GetMouseButton(0))
        {
            BasicAttack();
        }
    }
    #region DefaultVariables [SerializeField]

    [SerializeField] float DefaultACT = 0.45f;
   // [SerializeField] float DefaultS1CT = 3f;
   // [SerializeField] float DefaultS2CT = 4f;
   // [SerializeField] float DefaultS3CT = 7f;

    #endregion

    #region NetworkVariables

    public NetworkVariable<bool> OnDashColdown = new() { Value = false };
    public NetworkVariable<bool> BasicAttackcd = new() { Value = false };
    public NetworkVariable<bool> SpecAttack1cd = new() { Value = false };
    public NetworkVariable<bool> SpecAttack2cd = new() { Value = false };
    public NetworkVariable<bool> SpecAttack3cd = new() { Value = false };

    #endregion

    IEnumerator AttackCoroutine()
    {
        BasicAttackcd.Value = true;
        Debug.Log("OnSlash!");
        yield return new WaitForSecondsRealtime(DefaultACT);
        BasicAttackcd.Value = false;
        yield return null;
    }

    #region ServerAttacks 
    [ServerRpc]
    public void BasicAttackServerRpc(ulong UserID,Quaternion CameraRotation)
    {
        if ((!IsServer)||(BasicAttackcd.Value==true)) {return;}
        StartCoroutine(AttackCoroutine());
        GameObject Character = NetworkManager.SpawnManager.GetPlayerNetworkObject(UserID).transform.GetChild(0).GetChild(1).gameObject;
        GameObject Bullet = Instantiate(Character.GetComponent<User_SkinParams>().BasicAttackPrefab);
        Bullet.GetComponent<NetworkObject>().Spawn();
        Bullet.transform.SetPositionAndRotation(Character.transform.position+new Vector3(0f,0.55f,0f),CameraRotation);
        Bullet.GetComponent<Rigidbody>().velocity = Bullet.transform.forward *  2.5f;


    }
    #endregion

    #region LoaclAttacks 
    public void BasicAttack()
    {
        if ((!IsOwner)|| _Camera==null) return;
        
        BasicAttackServerRpc(OwnerClientId,_Camera.rotation);
        
    }
    #endregion
}
