using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _Player_ : MonoBehaviour
{
    #region _Variables_

    public int Level = 0;
    public int Exp = 0;
    public int Gold = 0;
    public string Name = "Null";
    [SerializeField]
    public List<_Character> Characters;

    #endregion
    public void Load()
    {
        Player_Datas _Datas = _LocalData_.Load_Player_Datas();

        Level = _Datas.Level;
        Exp = _Datas.Exp;
        Gold = _Datas.Gold;
        Name = _Datas.Name;
        Characters = _Datas.Characters;

        Debug.Log("Loaded");
    }
    public void Save()
    {
        _LocalData_.Save_Player_Datas(this);
        Debug.Log("Saved");
    }
}
