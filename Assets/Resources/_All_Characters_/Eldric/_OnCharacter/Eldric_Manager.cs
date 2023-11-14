using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Eldric_Manager : NetworkBehaviour
{
    public GameObject FindObjectByNetID(uint netID)
    {
        if (NetworkServer.spawned.TryGetValue(netID, out NetworkIdentity obj))
        {
            return obj.gameObject;
        }
        return null;
    }
    #region Vars
    [Header("Variables")]
    public float BasicHealth = 250f;
    public float BasicDefence = 10f;
    public float BasicPhysicPower = 1f;
    public float BasicMagicPower = 25f;
    public float BasicSpeed = 120f;
    public float BasicCrit = 0f;
    public float BasicCritDamage = 100f;
    [Header("0 - Человек . 1 - Нежить . 2 - Чудища . 3 - Приведение . 4 - Меха . 5 - Растения ")]
    public int BasicCharacterType = 0;
    _humanoid_ humanoid;
    [SerializeField]
    private GameObject _redbullet;
    #endregion
    private void Start()
    {
        try
        {
            humanoid = transform.parent.parent.GetComponent<_humanoid_>();
            humanoid.variables.MaxHealth = BasicHealth;
            humanoid.variables.Health = BasicHealth;
            humanoid.variables.Defence = BasicDefence;
            humanoid.variables.Speed = BasicSpeed;
            humanoid.variables.CritRarity = BasicCrit;
            humanoid.variables.CritPower = BasicCritDamage;
            humanoid.variables.PhysicPower = BasicPhysicPower;
            humanoid.variables.MagicPower = BasicMagicPower;
            humanoid.variables.Defence = BasicDefence;
            humanoid.variables.CharacterType = BasicCharacterType;
        }
        catch { Destroy(this.transform.gameObject);}
    }

    public void OnAttack()
    {
        DebouggerToText.DEBUGLOG("Do On Attack");
        try
        {
            DebouggerToText.DEBUGLOG("On Attack");
            CmdOnAttack(humanoid.transform.parent.GetComponent<NetworkIdentity>().netId);
            DebouggerToText.DEBUGLOG("On Attacked");
        }
        catch { return; }
    }
    [Command(requiresAuthority = false)]
    public void CmdOnAttack(uint NetworkID)
    {
        Debug.Log("SERVER ATTACK");
        if (!isServer) return;
        GameObject LocalCam = FindObjectByNetID(NetworkID).transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        Debug.Log($"Local Cam rotation:{LocalCam.transform.rotation}");
        GameObject newredbullet = Instantiate(_redbullet);
        newredbullet.transform.SetParent(FindObjectByNetID(NetworkID).transform.GetChild(1).transform);
        newredbullet.transform.position= LocalCam.transform.position+(Vector3.up*0.5f);
        newredbullet.transform.rotation = LocalCam.transform.rotation;
        newredbullet.GetComponent<_RedBulletScript>().Damage += LocalCam.transform.parent.parent.GetComponent<_humanoid_>().variables.MagicPower;
        newredbullet.GetComponent<_RedBulletScript>().Owner = NetworkID;
        NetworkServer.Spawn(newredbullet, FindObjectByNetID(NetworkID).gameObject);

    }
    [Command(requiresAuthority =false)]
    public void OnSpec1()
    {

    }
    [Command(requiresAuthority = false)]
    public void OnSpec2()
    {

    }
    [Command(requiresAuthority = false)]
    public void OnSpec3()
    {

    }
}
