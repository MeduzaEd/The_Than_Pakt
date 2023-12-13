using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Damage : NetworkBehaviour
{
    public ulong _OwnerID = ulong.MaxValue;
    public uint _DefaultDamage = 10;
    public List<uint> _Targeted = new();

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) { return; }
        // Обрабатывайте касание здесь
        if (collision.gameObject.CompareTag("Player"))
        {
           
            if (collision.transform.parent.GetComponent<Humanoid>()!=null)
            {
                Humanoid _Humanoid = collision.transform.parent.GetComponent<Humanoid>();
                _Humanoid.OnDamage(_OwnerID,_DefaultDamage);
            }
            Debug.Log("Объект с тегом YourTag столкнулся с этим объектом");
        }
    }
}
