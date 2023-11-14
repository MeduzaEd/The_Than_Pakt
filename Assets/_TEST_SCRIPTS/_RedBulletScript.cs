using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class _RedBulletScript : MonoBehaviour
{
    public Rigidbody _rb;
    public float Damage = 5f;
    public uint Owner = 0;
    private List <uint> Targeted;
    
    public _humanoid_ hum = null;
    public uint targetid = 0;
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

        try
        {
            hum = other.transform.parent.parent.GetComponent<_humanoid_>();
            if (hum.variables.Died == true || hum.variables.Inmortal == true|| hum.transform.parent.GetComponent<NetworkIdentity>()==null) { return; }
            targetid = hum.transform.parent.GetComponent<NetworkIdentity>().netId;
        }
        catch (Exception ex) { Debug.Log($"EXE:{ex}"); return; }
        Debug.Log($"Damaged:{other}");
        hum.OnDamage(Damage, Owner);
        Targeted.Add(targetid);
    }
}
