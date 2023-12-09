using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Eldric_Control : NetworkBehaviour
{
    #region DefaultVariables [SerializeField]

    [SerializeField] float DefaultACT = 0.45f;
    [SerializeField] float DefaultS1CT = 3f;
    [SerializeField] float DefaultS2CT = 4f;
    [SerializeField] float DefaultS3CT = 7f;

    #endregion

    #region NetworkVariables

    public NetworkVariable<bool> DashColdown = new NetworkVariable<bool>() { Value = false };
    public NetworkVariable<bool> BasicAttack = new NetworkVariable<bool>() { Value = false };
    public NetworkVariable<bool> SpecAttack1 = new NetworkVariable<bool>() { Value = false };
    public NetworkVariable<bool> SpecAttack2 = new NetworkVariable<bool>() { Value = false };
    public NetworkVariable<bool> SpecAttack3 = new NetworkVariable<bool>() { Value = false };

    #endregion


}
