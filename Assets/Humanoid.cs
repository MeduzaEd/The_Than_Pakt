using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
[System.Serializable]
public struct _Variables : INetworkSerializable
{
    public uint Health;
    [Range(500, 50000)]
    public uint MaxHealth;
    public uint Defence;
    [Range(10, 1200)]
    public uint Speed;
    public uint MagicPower;
    public uint PhysicPower;
    [Range(100, 350)]
    public uint AttackSpeed;
    [Range(0, 50)]
    public uint AttackColdown;
    [Range(0,100)]
    public uint CritRarity;
    [Range(150, 1000)]
    public uint CritPower;
    public bool Stun;
    public bool Stopped;
    public bool Died;
    public bool Inmortal;
    public uint PlayerKilled;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Health);
        serializer.SerializeValue(ref MaxHealth);
        serializer.SerializeValue(ref Defence);
        serializer.SerializeValue(ref Speed);
        serializer.SerializeValue(ref MagicPower);
        serializer.SerializeValue(ref PhysicPower);
        serializer.SerializeValue(ref AttackSpeed);
        serializer.SerializeValue(ref AttackColdown);
        serializer.SerializeValue(ref CritRarity);
        serializer.SerializeValue(ref CritPower);
        serializer.SerializeValue(ref Stun);
        serializer.SerializeValue(ref Stopped);
        serializer.SerializeValue(ref Died);
        serializer.SerializeValue(ref Inmortal);
        serializer.SerializeValue(ref PlayerKilled);
    }
}


public class Humanoid : MonoBehaviour
{
    [SerializeField]  uint StartHealth = 2000;

    NetworkManager l_networkManager;
    private void Start()
    {
        l_networkManager = GameObject.FindObjectOfType<NetworkManager>();
        if (!l_networkManager.IsServer) { return; }
        _Variables newVariables = variables.Value;
        newVariables.MaxHealth = StartHealth;
        variables.Value = newVariables;
    }



    public NetworkVariable<_Variables> variables = new NetworkVariable<_Variables>(
        new _Variables
        {
            Health= 1000,
            MaxHealth = 1000,
            AttackSpeed = 100,
            Defence = 0,
            MagicPower = 60,
            PhysicPower = 120,
            CritPower = 100,
            CritRarity = 0,
            Speed = 100,


        },
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );

}
