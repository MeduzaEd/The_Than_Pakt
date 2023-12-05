using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Eldric_Control : NetworkBehaviour
{
    #region NetworkVariables

    public NetworkVariable<bool> DashColdown=new NetworkVariable<bool>();
    public NetworkVariable<bool> BasicAttack = new NetworkVariable<bool>();
    public NetworkVariable<bool> SpecAttack1 = new NetworkVariable<bool>();
    public NetworkVariable<bool> SpecAttack2 = new NetworkVariable<bool>();
    public NetworkVariable<bool> SpecAttack3 = new NetworkVariable<bool>();

    #endregion
}
