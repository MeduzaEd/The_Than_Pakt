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
    public void LoadCharacterServerRpc(string CharacterPath,string SkinPath)
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
            }
            SpawnCharacter.GetComponent<NetworkObject>().Spawn();
            SpawnSkin.GetComponent<NetworkObject>().Spawn();
            SpawnCharacter.transform.SetParent(this.transform);
            SpawnSkin.transform.SetParent(SpawnCharacter.transform);
            
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
    private _Cache_Save_System_ _CSS;
    private void Start()
    {
        Debug.Log("1");
        if(IsOwner&&IsClient)
        {
            Debug.Log("2");
            _CSS = _Cache_Save_System_.FindObjectOfType<_Cache_Save_System_>();
            LoadCharacterServerRpc(_CSS.UserData.SelectedCharacterPath, _CSS.UserData.SelectedCharacterSkinPath);
            Debug.Log("end");
        }
    }
}
