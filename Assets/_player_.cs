using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class _player_ : NetworkBehaviour
{
    [SyncVar]
    public string Character_Path = "Assets/_All_Characters_/Eldric/_OnCharacter/Character.prefab";
    [SyncVar]
    public string Skin_Path = "Assets/_All_Characters_/Eldric/sunrise/Character.prefab";


}
