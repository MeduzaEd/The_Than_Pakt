using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class _Server_Scripts_ : NetworkBehaviour
{
    //[SyncVar]
    // private int serverValue = 0;
    #region _Prefabs_
    [Header("_Prefabs_"), SerializeField]
    private GameObject Characters_Parent;
    [SerializeField, Range(0.005f, 0.225f)]
    private float SpawnRate = 0.02f;
    [SerializeField]
    private GameObject Character_Prefab;
    #endregion


    [Command(requiresAuthority = false)]
    public void CreateCharacter(string CharacterId,string CharacterName)
    {
        Debug.Log("Character ADD");
        CreatePlayer(CharacterId, CharacterName);
    }


    #region /// ___SERVER___ \\\
    [Server]
    public void CreatePlayer(string CharacterId, string CharacterName)
    {
        if (string.IsNullOrEmpty(CharacterId) || string.IsNullOrEmpty(CharacterName) || CharacterName.Length < 3 || CharacterId.Length < 7)
        {
            return;
        }

        if (Characters_Parent.transform.Find(CharacterId) == null)
        {
            Debug.Log("Character ADDED");
            GameObject newPlayer = Instantiate(Character_Prefab, Characters_Parent.transform);
            GameObject.FindGameObjectWithTag("_USERS_").transform.Find(CharacterId).GetComponent<_User_Script_>().Character= newPlayer;

            newPlayer.transform.position = new Vector3(Random.Range(-SpawnRate, SpawnRate),0, Random.Range(-SpawnRate, SpawnRate)); // Устанавливаем начальную позицию, например, в точку (0, 0, 0).
            newPlayer.name = CharacterId;
            NetworkServer.Spawn(newPlayer);
        }
    }
    #endregion
}
