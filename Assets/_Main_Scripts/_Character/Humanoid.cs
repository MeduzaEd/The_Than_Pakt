using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;



public class Humanoid : NetworkBehaviour
{
   // public void OnDamage(float Damage,uint Killer)
   // {
  //      if (!IsServer) { return; }

  //  }
  
    public void Start()
    {
        Debug.Log("HumanoidStarted!!");
    }
    #region NetVars
    public NetworkVariable<uint> Health = new() { Value = 1000 };
    public NetworkVariable<uint> MaxHealth = new() { Value = 1000 };
    public NetworkVariable<uint> Defence = new() { Value = 10 };
    public NetworkVariable<uint> Speed = new() { Value = 10 };
    public NetworkVariable<uint> MagicPower = new() { Value = 1 };
    public NetworkVariable<uint> PhysicPower = new() { Value = 1 };
    public NetworkVariable<uint> AttackSpeed = new() { Value = 1 };
    public NetworkVariable<uint> AttackColdown = new() { Value = 1 };
    public NetworkVariable<uint> CritRarity = new() { Value = 1 };
    public NetworkVariable<uint> CritPower = new() { Value = 1 };
    public NetworkVariable<uint> Stun;
    public NetworkVariable<uint> Stopped;
    public NetworkVariable<ulong> PlayerKilled = new() { Value = ulong.MaxValue };
    public NetworkVariable<bool> Inmortal;
    public NetworkVariable<bool> Died;
    public NetworkVariable<bool> Block;
    public NetworkVariable<bool> OnAttack;
    #endregion
    public void OnDamage(ulong _OwnerID,uint _Damage)
    {

        if (!IsServer) { return; }
        Debug.LogWarning("OnDamaged");
        if(Health.Value == 0) { return; }
        PlayerKilled.Value = _OwnerID;
        if (Health.Value< _Damage) { Health.Value = 0; }
        else { Health.Value -= _Damage; }
    }
    // public List<NetworkVariable<uint>> Sollowing;
    public NetworkVariable<uint> SpeedMulti = new(){ Value = 100};
}
