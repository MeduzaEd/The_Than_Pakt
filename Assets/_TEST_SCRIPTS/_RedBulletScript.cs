using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class _RedBulletScript : MonoBehaviour
{
    public Rigidbody _rb;
    public float Damage = 5f;
    public uint Owner = 0;
    private List <uint> Targeted;
    
    _humanoid_ hum = null;
    uint targetid = 0;
    private void Start()
    {
        StartCoroutine(OnStart(99f));
    }
    IEnumerator OnStart(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        _rb.velocity = transform.up * 4f;
        yield return null;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.ToString());
        
        try{hum = other.transform.parent.parent.GetComponent<_humanoid_>(); targetid = hum.transform.parent.GetComponent<NetworkIdentity>().netId; }
        catch { return; }
        if(hum.variables.Died==true||hum.variables.Inmortal==true||Targeted.Contains(targetid)){return;}
        hum.OnDamage(Damage, Owner, targetid);
        Targeted.Add(targetid);
    }
}
