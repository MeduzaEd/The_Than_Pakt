using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
[System.Serializable]
public struct Variables
{
    [SyncVar]
    public string Character;
    [SyncVar]
    public string SkinPath;
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
    public bool Stun;
    [SyncVar]
    public bool Stopped;
    [SyncVar]
    public bool Pushed;

}

public class _humanoid_ : MonoBehaviour
{
    public Variables variables;
    private void Start()
    {
        
    }

}
