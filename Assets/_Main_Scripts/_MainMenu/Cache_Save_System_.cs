using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


[System.Serializable]
public struct AllUserData
{


    #region User Main Data's

    public uint Golds;
    public uint Crystals;

    #endregion

    #region User Characters Data's

    public List<Character> Characters;

    public string SelectedCharacterPath;
    public string SelectedCharacterSkinPath;
    #endregion

    #region Menu Data's
    public uint TutorialStage;
    public bool SoundsIsMute;
    public float SoundsVolume;
    public int MaxUsersInHost;
    public string MyServerName;
    #endregion
}
[System.Serializable]
public struct Character
{
    public string CharacterPath;
    public List<string> CharacterSkins;
}
public class Cache_Save_System_ : MonoBehaviour
{
    public AllUserData UserData=new();
    public static Cache_Save_System_ _SaveSingeton;
    [SerializeField] string StarterCharacterPath;
    [SerializeField] string StarterSkinPath;
    public void SaveData()
    {
    
        string encryptedJson = EncryptionManager.Encrypt(JsonUtility.ToJson(UserData));
        string directoryPath = Path.Combine(Application.persistentDataPath, "MeduzaEdCompany", "Soul_Night");
        string filePath = Path.Combine(directoryPath, "IsUserLocalData.json");

        // Создаем директорию, если ее нет
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Проверяем, существует ли файл
        if (File.Exists(filePath))
        {
            // Если существует, обновляем его
            File.WriteAllText(filePath, encryptedJson);
            Debug.Log("Data updated at: " + filePath);
        }
        else
        {
            // Если файла нет, создаем новый
            File.WriteAllText(filePath, encryptedJson);
            Debug.Log("Data saved at: " + filePath);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        _SaveSingeton = this;
        LoadData();
        CheckToLoad();
        CheckToSelected();
    }
    public void CheckToLoad()
    {
        if(UserData.Characters.Count <=0)
        {
            Character _Character = new();
            _Character.CharacterPath = StarterCharacterPath.ToString();
            _Character.CharacterSkins = new();
            _Character.CharacterSkins.Add(StarterSkinPath);
            UserData.Characters.Add(_Character);
        }
    }
    private void CheckToSelected()
    {
        try
        {
            GameObject _object = Instantiate(Resources.Load<GameObject>(UserData.SelectedCharacterPath));
            NetworkObject _networkobject = _object.GetComponent<NetworkObject>();
            Destroy(_object);
            GameObject _skinobject = Instantiate(Resources.Load<GameObject>(UserData.SelectedCharacterSkinPath));
            NetworkObject _skinnetworkobject = _skinobject.GetComponent<NetworkObject>();
            Destroy(_skinobject);
        }
        catch
        {
            UserData.SelectedCharacterPath = StarterCharacterPath;
            UserData.SelectedCharacterSkinPath = StarterSkinPath;
        }
    }
    public void LoadData()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "MeduzaEdCompany", "Soul_Night");
        string filePath = Path.Combine(directoryPath, "IsUserLocalData.json");
        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            if (File.Exists(filePath))
            {
        
                string decryptedJson = EncryptionManager.Decrypt(File.ReadAllText(filePath));
                AllUserData data = JsonUtility.FromJson<AllUserData>(decryptedJson);
               // JsonUtility.FromJsonOverwrite(decryptedJson, UserData);
                UserData= data;
                #region Sync to start!
                GameObject.FindObjectOfType<User_Interface>().ScroolVolumeSound.value = UserData.SoundsVolume;
                GameObject.FindObjectOfType<User_Interface>().ImageChange();
                GameObject.FindObjectOfType<User_Interface>().ScroolVolumeMaxConnections.value =((float)UserData.MaxUsersInHost)/16;
                GameObject.FindObjectOfType<User_Interface>().MaxUsersChange();
                GameObject.FindObjectOfType<User_Interface>().ScroolVolumeMaxConnections.onValueChanged.AddListener(The_MaxUsersChange);
                GameObject.FindObjectOfType<User_Interface>().ServerName.transform.parent.GetComponent<InputField>().text = UserData.MyServerName ;
                GameObject.FindObjectOfType<User_Interface>().ServerNameChange(UserData.MyServerName);

                #endregion
                Debug.Log("UserLocalData loaded from: " + UserData.MaxUsersInHost);
            }
            else
            {

                SaveData();
                Debug.Log("Save file-'UserLocalData' not found and Saved new file to path : " + filePath);
            }
        }catch(Exception ex) { Debug.Log(ex);}
    }

    private void The_MaxUsersChange(float _)
    {
        GameObject.FindObjectOfType<User_Interface>().MaxUsersChange();
    }
    private void OnApplicationQuit()
    {
        SaveData();
        Debug.Log($"{ UserData.MaxUsersInHost} - HasSaved!");
    }
}
