using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Damage : NetworkBehaviour
{
    public ulong _OwnerID = ulong.MaxValue;
    public uint _DefaultDamage = 10;
    public List<uint> _Targeted = new();

    private void OnTriggerEnter(Collider collision)
    {
        if (!IsServer) { return; }
        //Debug.Log($"this:{_OwnerID},damage:{_DefaultDamage}");

        if (collision.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            Transform parentTransform = collision.gameObject.transform;

            if (parentTransform != null)
            {
                NetworkObject networkObject = parentTransform.GetComponent<NetworkObject>();

                if (networkObject != null)
                {
                    ulong networkObjectId = networkObject.NetworkObjectId;
                    Debug.LogWarning("Damage");
                    TheDamage(_OwnerID, networkObjectId, _DefaultDamage);
                }
                else
                {
                    Debug.LogWarning("Компонент NetworkObject не найден на родительском трансформе.");
                }
            }
            else
            {
                Debug.LogWarning("Родительский трансформ не найден у объекта столкновения.");
            }
        }
    }

    //[ServerRpc]
    private void TheDamage(ulong _MyId, ulong _Id, uint _Dmg)
    {
        if (!IsServer) { return; }
        Debug.Log($"Attempting to invoke TheDamageServerRpc for object with ID {_Id}");

        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(_Id, out var networkObject))
        {
            Debug.Log($"Found object with ID {_Id}");

            Humanoid _Humanoid = networkObject.GetComponent<Humanoid>();
            if(_Humanoid ==null)
            {
                _Humanoid=networkObject.gameObject.transform.parent.GetComponent<Humanoid>();
            }
            if (_Humanoid != null)
            {
                Debug.Log("Found Humanoid component on the network object.");

                _Humanoid.OnDamage(_MyId, _Dmg);
            }
            else
            {

                Debug.LogWarning("Humanoid component not found on the network object.");
            }
        }
        else
        {
            Debug.LogWarning($"Object with ID {_Id} not found.");
        }
    }

}
