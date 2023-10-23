using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class _Server_Scripts_ : NetworkBehaviour
{
    #region _Prefabs_
    [Header("_Prefabs_"), SerializeField]
    private GameObject Characters_Parent;
    [SerializeField, Range(0.005f, 0.225f)]
    private float SpawnRate = 0.02f;
    [SerializeField]
    private GameObject Character_Prefab;
    #endregion

    #region /// ___Commands___ \\\
    [Command(requiresAuthority = false)]
    public void CreateCharacter(GameObject User, string CharacterId,string CharacterName)
    {
        if (!NetworkServer.active) { Debug.Log("NetworkServer not active");  return; }
        Debug.Log("Character ADD");
        CreatePlayer(User,CharacterId, CharacterName);

    }
    #endregion

    #region /// ___SERVER___ \\\
    [Server]
    public void CreatePlayer(GameObject User, string CharacterId, string CharacterName)
    {
        if (string.IsNullOrEmpty(CharacterId) || string.IsNullOrEmpty(CharacterName) || CharacterName.Length < 3 || CharacterId.Length < 7)
        {
            Debug.Log("Character Not ADDED");
            return;
        }
        Debug.Log("Character ADDED");
        GameObject newPlayer = Instantiate(Character_Prefab, Characters_Parent.transform);
        User.GetComponent<_User_Script_>().Character = newPlayer;

        newPlayer.transform.position = new Vector3(Random.Range(-SpawnRate, SpawnRate), 0, Random.Range(-SpawnRate, SpawnRate)); // Устанавливаем начальную позицию, например, в точку (0, 0, 0).
        newPlayer.name = CharacterId;
        NetworkServer.Spawn(newPlayer);
       
    }
    #endregion
}
