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
    Transform _camera;
    [SerializeField]
    private GameObject _redbullet;
    #endregion
    private void Start()
    {
        try
        {
            _camera = GameObject.FindObjectOfType<Camera>().transform;
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
            CmdOnAttack(humanoid.transform.parent.GetComponent<NetworkIdentity>().netId, _camera.rotation.eulerAngles.y);
            DebouggerToText.DEBUGLOG("On Attacked");
        }
        catch { return; }
    }
    [Command(requiresAuthority = false)]
    public void CmdOnAttack(uint NetworkID, float cruay)
    {
        Debug.Log("SERVER ATTACK");
        if (!isServer) return;
        GameObject newredbullet = Instantiate(_redbullet);
        newredbullet.transform.SetParent(FindObjectByNetID(NetworkID).transform.GetChild(1).transform);
        Transform Character = FindObjectByNetID(NetworkID).transform.GetChild(0).GetChild(0).GetChild(1).transform;
        newredbullet.transform.position = Character.position+(Vector3.up*0.5f)+(Character.right*(Random.Range(-0.3f,0.3f)));
        newredbullet.transform.rotation = Quaternion.Euler(0, cruay, 0);
        newredbullet.transform.GetChild(0).GetComponent<_RedBulletScript>().Damage += FindObjectByNetID(NetworkID).transform.GetChild(0).GetComponent<_humanoid_>().variables.MagicPower;
        newredbullet.transform.GetChild(0).GetComponent<_RedBulletScript>().Owner = NetworkID;
        newredbullet.transform.localScale = new Vector3(3f, 3f, 3f);
        NetworkServer.Spawn(newredbullet, FindObjectByNetID(NetworkID).gameObject);
        Debug.Log($"Local Cam rotation:{newredbullet.transform.rotation}");

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
