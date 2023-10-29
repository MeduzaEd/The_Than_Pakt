#region _Using_
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Mirror;
#endregion
// ________________________ \\
#region _Data_
[System.Serializable]
public class Player_Datas
{
    [SyncVar]
    public int Level;
    [SyncVar]
    public int Exp;
    [SyncVar]
    public int Gold;
    [SyncVar]
    public string Name;
    public List<_Character> Characters;
    public Player_Datas (_Player_ User)
    {
        Level = User.Level;
        Exp = User.Exp;
        Gold = User.Gold;
        Name = User.Name;
        Characters = User.Characters;
    }
}
[System.Serializable]
public struct _Character
{
    [SyncVar]
    public string Name;
    public List<_Character_Skin> Character_Skins;
}
[System.Serializable]
public struct _Character_Skin
{
    [SyncVar]
    public string Name;
}
#endregion
// ________________________ \\
#region _Script_
public static class _LocalData_ 
{

    private static string _Path_ = "/_UserData.fun";
    #region _Save_
    public static void Save_Player_Datas(_Player_ _user)
    {
        BinaryFormatter _Formatter = new BinaryFormatter();
        string _Path = Application.persistentDataPath + _Path_;
        FileStream _Stream = new FileStream(_Path, FileMode.Create);

        Player_Datas _Data = new Player_Datas(_user);

        _Formatter.Serialize(_Stream, _Data);
        _Stream.Close();
    }
    #endregion
// ________________________ \\
    #region _Load_
    public static Player_Datas Load_Player_Datas()
    {
        string _Path = Application.persistentDataPath + _Path_;
        if(File.Exists(_Path))
        {
            BinaryFormatter _Formatter = new BinaryFormatter();
            FileStream _Stream = new FileStream(_Path, FileMode.Open);

            Player_Datas _Datas = _Formatter.Deserialize(_Stream) as Player_Datas;

            _Stream.Close();

            return _Datas;

        } else { return null; }

    }
    #endregion
}
#endregion
// ________________________ \\