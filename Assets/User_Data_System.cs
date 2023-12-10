using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class User_Data_System : NetworkBehaviour
{
    public void TryDestroy(GameObject _obj)
    {
        if (_obj != null)
        {
            Destroy(_obj);
        }
    }
    [ServerRpc]
    public void LoadCharacterServerRpc(string CharacterPath,string SkinPath,ulong UserId)
    {
        Debug.Log($"s1CharacterPath: {CharacterPath} and SkinPath:{SkinPath}");
        if (!IsServer) {return;}
        Debug.Log("s2");
        GameObject SpawnCharacter=null;
        GameObject SpawnSkin=null;
        try 
        {
            Debug.Log($"CharacterPath0: {Resources.Load<GameObject>(CharacterPath)}");
            Debug.Log($"CharacterPath1: {Instantiate(Resources.Load<GameObject>(CharacterPath))}");
            SpawnCharacter =Instantiate(Resources.Load<GameObject>(CharacterPath));
            Debug.Log($"SpawnCharacter: {SpawnCharacter}");
            SpawnSkin = Instantiate(Resources.Load<GameObject>(SkinPath));
            Debug.Log($"SpawnCharacter: {SpawnCharacter} and SpawnSkin:{SpawnSkin}");
            if (SpawnCharacter.GetComponent<Character_Type>().Type!= SpawnSkin.GetComponent<Character_Type>().Type)
            {
                TryDestroy(SpawnCharacter);
                TryDestroy(SpawnSkin);
                Debug.Log("Dont spawn!!!");
                return;
            }
            
            SpawnCharacter.GetComponent<NetworkObject>().SpawnWithOwnership(UserId);
            SpawnSkin.GetComponent<NetworkObject>().SpawnWithOwnership(UserId);
            SpawnCharacter.GetComponent<NetworkObject>().TrySetParent(NetworkManager.SpawnManager.GetPlayerNetworkObject(UserId).transform);
            SpawnSkin.GetComponent<NetworkObject>().TrySetParent(SpawnCharacter.transform);
            SpawnCharacter.transform.position = Vector3.zero;
            SpawnSkin.transform.position = Vector3.zero;
          
       
            //_rb = GetComponentInChildren<Rigidbody>();
            //_camera = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Camera>();

            SpawnCharacter.transform.parent.GetComponent<User_Control>().UserLoaded.Value=true;
            Debug.Log("sSpawn");
        }
        catch(Exception ex)
        {
            TryDestroy(SpawnCharacter);
            TryDestroy(SpawnSkin);
            Debug.Log($"sdestroy with:{ex}");
            return;
        }
        Debug.Log("send");
    }
    private Cache_Save_System_ _CSS;
    private void Start()
    {
        Debug.Log("1");
        if(IsOwner&&IsClient&&IsLocalPlayer)
        {
            Debug.Log("2");
            _CSS = Cache_Save_System_.FindObjectOfType<Cache_Save_System_>();
            LoadCharacterServerRpc(_CSS.UserData.SelectedCharacterPath, _CSS.UserData.SelectedCharacterSkinPath,OwnerClientId);
            Debug.Log("end");
        }
    }
}
