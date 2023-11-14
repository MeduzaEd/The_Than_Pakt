using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class _RedBulletScript : NetworkBehaviour
{
    public Rigidbody _rb;
    public float Damage = 5f;
    public uint Owner = 0;
   // private List <uint> Targeted;
    
    public _humanoid_ hum = null;
    public uint targetid = 0;

    private void Start()
    {
        if (NetworkServer.spawned.TryGetValue(Owner, out NetworkIdentity obj))
        {
            CmdStart(Owner,GetComponent<NetworkIdentity>().netId);
        }
        
    }
    IEnumerator OnStart(float _time)
    {
        _rb.velocity = transform.right * UnityEngine.Random.Range(-0.05f,0.05f);
        yield return new WaitForSecondsRealtime(_time);
        _rb.velocity = Vector3.zero;
       _rb.velocity = transform.forward * 9f;
        yield return null;
        yield return new WaitForSecondsRealtime(_time);
        //Debug.Log("DESTROOY");
        CmdDestroy(netId);
        
    }
    [Command(requiresAuthority =false)]
    public void CmdDestroy(uint Id)
    {
        if (!isServer) return;
        if (NetworkServer.spawned.TryGetValue(Id, out NetworkIdentity obj))
        {
          
            NetworkServer.Destroy(obj.gameObject);
        }

    }
    [Command(requiresAuthority = false)]
    public void CmdStart(uint Id,uint thisid)
    {
        if (!isServer) return;
        if (NetworkServer.spawned.TryGetValue(Id, out NetworkIdentity obj)&& NetworkServer.spawned.TryGetValue(thisid, out NetworkIdentity thisobj))
        {
            thisobj.transform.SetParent(obj.transform.GetChild(1).transform);
        }
        StartCoroutine(OnStart(9f));
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
        //Targeted.Add(targetid);
    }
}
