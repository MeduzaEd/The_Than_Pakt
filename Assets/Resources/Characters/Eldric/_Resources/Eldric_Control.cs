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
    IEnumerator WaitRealTime(float _Time)
    {
        yield return new WaitForSecondsRealtime(_Time);
        yield return null;
    }
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
        for (int i=-1; i<2;i++)
        {
            GameObject Character = NetworkManager.SpawnManager.GetPlayerNetworkObject(UserID).transform.GetChild(0).GetChild(1).gameObject;
            GameObject Bullet = Instantiate(Character.GetComponent<User_SkinParams>().BasicAttackPrefab);
            Bullet.GetComponent<NetworkObject>().Spawn();
            Bullet.transform.SetPositionAndRotation(Character.transform.position + new Vector3(0, Random.Range(.95f, 1.125f)*0.55f,0)+(Random.Range(.9f, 3.5f) * 0.1f *i*Character.transform.right) + (Random.Range(-.2f,.2f)* Character.transform.forward), CameraRotation);
            Bullet.GetComponent<Rigidbody>().AddForce(Bullet.transform.forward * 15f, ForceMode.Impulse);
            StartCoroutine(WaitRealTime(Random.Range(.095f, 0.155f)));
        }

        //  Bullet.transform.SetPositionAndRotation(Character.transform.position+new Vector3(0f,0.55f,0f),Quaternion.LookRotation(Character.transform.forward * 40f,Vector3.up));
    }
    #endregion

    #region LoaclAttacks 
    public void BasicAttack()
    {
        if ((!IsOwner)|| _Camera==null) return;
        
        BasicAttackServerRpc(OwnerClientId,Quaternion.LookRotation((33f* _Camera.transform.forward)+(7f*_Camera.transform.up)+new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)),Vector3.up));
        
    }
    #endregion
}
