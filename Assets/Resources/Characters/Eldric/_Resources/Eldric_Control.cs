using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Eldric_Control : NetworkBehaviour
{

    private void Start()
    {
        if (!IsOwner) return;

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
    public void BasicAttackServerRpc(ulong UserID)
    {
        if ((!IsServer)||(BasicAttackcd.Value==true)) {return;}
        Debug.Log($"1OnSlash:{UserID}");
        StartCoroutine(AttackCoroutine());

    }
    #endregion

    #region LoaclAttacks 
    public void BasicAttack()
    {
        if (!IsOwner) return;
        BasicAttackServerRpc(OwnerClientId);
        Debug.Log("2OnSlash!");
    }
    #endregion
}
