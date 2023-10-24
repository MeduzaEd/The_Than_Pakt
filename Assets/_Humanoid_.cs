using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
[System.Serializable]
public struct HumanoidStats
{
    public int Health;
    public int MaxHealth;
    public int Mana;
    public int MaxMana;
    public int Defence;
    public int Speed;
    public bool Died;
}
public class _Humanoid_ : NetworkBehaviour
{
    [SyncVar]
    public HumanoidStats Stats;
    public _Player_ Killer;
    public _Player_ _User;
    public void OnDamage(_Player_ _Killer_,int _Damage)
    {
        if (!isServerOnly) { return; }
        Debug.Log("OnDamaged");
    }
}
