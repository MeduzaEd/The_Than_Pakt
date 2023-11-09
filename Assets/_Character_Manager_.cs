using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
public class _Character_Manager_ : NetworkBehaviour
{
    private _player_ player;
    private GameObject character;
    
    private void Start()
    {
        player = this.transform.parent.parent.GetComponent<_player_>();
        //Spawn
        character =Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(player.Character_Path));
        GameObject Skin = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(player.Skin_Path));
        Skin.transform.parent =character.transform;
        character.transform.parent= this.transform;
        //Spawn On Network
        CharacterSpawn();
    }
    [Command]
    private void CharacterSpawn()
    {
        NetworkServer.Spawn(character);
    }
}
