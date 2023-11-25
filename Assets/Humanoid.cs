using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;



public class Humanoid : NetworkBehaviour
{
    public void OnDamage(float Damage,uint Killer)
    {
        if (!IsServer) { return; }

    }

    public NetworkVariable<uint> Health;
    public NetworkVariable<uint> MaxHealth;
    public NetworkVariable<uint> Defence;
    public NetworkVariable<uint> Speed;
    public NetworkVariable<uint> MagicPower;
    public NetworkVariable<uint> PhysicPower;
    public NetworkVariable<uint> AttackSpeed;
    public NetworkVariable<uint> AttackColdown;
    public NetworkVariable<uint> CritRarity;
    public NetworkVariable<uint> CritPower;
    public NetworkVariable<uint> Stun;
    public NetworkVariable<uint> Stopped;
    public NetworkVariable<uint> PlayerKilled;
    public NetworkVariable<bool> Inmortal;
    public NetworkVariable<bool> Died;
    public NetworkVariable<bool> Block;
}
