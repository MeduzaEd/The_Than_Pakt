using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class _player_ : NetworkBehaviour
{
    [SyncVar]
    public string Character_Path = "_All_Characters_/Eldric/_OnCharacter/Character";
    [SyncVar]
    public string Skin_Path = "_All_Characters_/Eldric/sunrise/Character";


}
