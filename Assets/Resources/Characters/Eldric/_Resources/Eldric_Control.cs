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

    [SerializeField] float DefaultACT = 0.35f;
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

    IEnumerator AttackCoroutine(ulong UserID)
    {
        Humanoid _Humanoid = NetworkManager.SpawnManager.GetPlayerNetworkObject(UserID).GetComponent<Humanoid>();
        BasicAttackcd.Value = true;
        
        _Humanoid.OnAttack.Value = true;
        Debug.Log("OnSlash!");
        yield return new WaitForSecondsRealtime(DefaultACT);
        BasicAttackcd.Value = false;
        yield return new WaitForSecondsRealtime(0.1f);
        if (_Humanoid.OnAttack.Value == true && BasicAttackcd.Value == false)
        { _Humanoid.OnAttack.Value = false; }
        yield return null;
    }
    IEnumerator RotationgCorutione(ulong UserID, Quaternion CameraRotation)
    {
        Rigidbody _Rb = NetworkManager.SpawnManager.GetPlayerNetworkObject(UserID).transform.GetChild(0).GetComponent<Rigidbody>();
        Vector3 moveDirection = new((CameraRotation * _Rb.transform.forward).x, 0, (CameraRotation * _Rb.transform.forward).z);
        // Остальной код остается без изменений

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        _Rb.transform.GetChild(2).transform.rotation = Quaternion.Slerp(_Rb.transform.GetChild(2).transform.rotation, targetRotation,  10f);

        yield return null;
    }
        #region ServerAttacks 
    [ServerRpc]
    public void BasicAttackServerRpc(ulong UserID,Quaternion CameraRotation)
    {
        if ((!IsServer)||(BasicAttackcd.Value==true)) {return;}
        StartCoroutine(AttackCoroutine(UserID));
        StartCoroutine(RotationgCorutione(UserID, CameraRotation));
        int _I = Random.Range(2, 3);
        int _UpDown = Random.Range(0, 1);
        _UpDown = _UpDown > 0 ? 1 : -1;
        Debug.Log(_I);
        Debug.Log(_UpDown);
        for (int i = _UpDown * (_I); _UpDown == 1 ? i > -1 * _I : i < _I; i += -1 * _UpDown)
        {
            Debug.Log(i);
            StartCoroutine(SpawnBulletsWithDelay(Random.Range(.45f, 0.75f), UserID, CameraRotation,i));

        }
    }

    IEnumerator SpawnBulletsWithDelay(float delay, ulong UserID, Quaternion CameraRotation,int i)
    {
        yield return new WaitForSeconds(delay);
        GameObject Character = NetworkManager.SpawnManager.GetPlayerNetworkObject(UserID).transform.GetChild(0).GetChild(3).gameObject;
        GameObject Bullet = Instantiate(Character.GetComponent<User_SkinParams>().BasicAttackPrefab);
        Bullet.GetComponent<NetworkObject>().SpawnWithOwnership(UserID);
        Bullet.transform.SetPositionAndRotation(Character.transform.position + new Vector3(0, (Random.Range(-1, 1) * Random.Range(.055f, .125f) * i) + 0.75f, 0) + (Random.Range(.9f, 2.5f) * 0.1f * i * Character.transform.right) + (Random.Range(-.2f, .2f) * Character.transform.forward), CameraRotation);
        Bullet.GetComponent<Damage>()._OwnerID = UserID;
        Bullet.GetComponent<Damage>()._DefaultDamage += NetworkManager.SpawnManager.GetPlayerNetworkObject(UserID).GetComponent<Humanoid>().MagicPower.Value;
        yield return null;
    }
    #endregion

    #region LoaclAttacks 
    public void BasicAttack()
    {
        if ((!IsOwner)|| _Camera==null) return;
        Debug.Log($"OwnerClientId:{OwnerClientId}");
        BasicAttackServerRpc(OwnerClientId,Quaternion.LookRotation((33f* _Camera.transform.forward)+(7f*_Camera.transform.up)+new Vector3(Random.Range(-.3f, .5f), Random.Range(-.5f, .3f), Random.Range(-.3f, .6f)),Vector3.up));
        
    }
    #endregion
}
