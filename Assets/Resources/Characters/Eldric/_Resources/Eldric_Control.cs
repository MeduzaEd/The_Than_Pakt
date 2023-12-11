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
        StartCoroutine(SpawnBulletsWithDelay(Random.Range(.095f, 0.155f),UserID,CameraRotation));
    }

    IEnumerator SpawnBulletsWithDelay(float delay, ulong UserID, Quaternion CameraRotation)
    {
        for (int i = Random.Range(-2, 0); i < Random.Range(0, 3); i++)
        {
            GameObject Character = NetworkManager.SpawnManager.GetPlayerNetworkObject(UserID).transform.GetChild(0).GetChild(1).gameObject;
            GameObject Bullet = Instantiate(Character.GetComponent<User_SkinParams>().BasicAttackPrefab);
            Bullet.GetComponent<NetworkObject>().SpawnWithOwnership(UserID);
            Bullet.transform.SetPositionAndRotation(Character.transform.position + new Vector3(0, (Random.Range(-1, 1) * Random.Range(.055f, .125f)* i) + 0.55f, 0) + (Random.Range(.9f, 3.5f) * 0.1f * i * Character.transform.right) + (Random.Range(-.2f, .2f) * Character.transform.forward), CameraRotation);

            yield return new WaitForSeconds(delay);
        }
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
