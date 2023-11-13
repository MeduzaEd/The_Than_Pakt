using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
[System.Serializable]
public struct Variables
{
    [SyncVar]
    public float Health;
    [SyncVar]
    public float MaxHealth;
    [SyncVar]
    public float Defence;
    [SyncVar]
    public float PhysicPower;
    [SyncVar]
    public float MagicPower;
    [SyncVar]
    public float Speed;
    [SyncVar]
    public float CritRarity;
    [SyncVar]
    public float CritPower;
    [SyncVar]
    public float AttackSpeed;
    [SyncVar]
    public int CharacterType;
    [SyncVar]
    public bool DontStun;
    [SyncVar]
    public bool DontStop;
    [SyncVar]
    public bool Inmortal;
    [SyncVar]
    public bool Stun;
    [SyncVar]
    public bool Stopped;
    [SyncVar]
    public bool Pushed;//Толчек как в млбб от Огненного выстрела или подброс вейла
    [SyncVar]
    public bool Died;
    [SyncVar]
    public uint KilledMe;
    public bool IsPlayer;
}

public class _humanoid_ : NetworkBehaviour
{
    public GameObject FindObjectByNetID(uint netID)
    {
        if (NetworkServer.spawned.TryGetValue(netID, out NetworkIdentity obj))
        {
            return obj.gameObject;
        }
        return null;
    }
    public Variables variables;
    
    [Command(requiresAuthority =false)]
    public void OnDamage(float Damage,uint ThisID, uint EnemyID)
    {
        if (Damage <= 0|| FindObjectByNetID(ThisID)==null || FindObjectByNetID(EnemyID) == null) { return; }
        _humanoid_ enemyhumanoid = FindObjectByNetID(EnemyID).GetComponentInChildren<_humanoid_>();
        if(enemyhumanoid.variables.Died == true||enemyhumanoid.variables.Inmortal==true) { return; }
        enemyhumanoid.variables.Health -= Damage;
        enemyhumanoid.variables.KilledMe = ThisID;
    }

}
