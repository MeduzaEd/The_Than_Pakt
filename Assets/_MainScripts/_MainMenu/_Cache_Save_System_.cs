using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[System.Serializable]
public struct AllUserData
{


    #region User Main Data's

    public uint Wind_Jade;
    public uint Crystals;

    #endregion

    #region User Characters Data's

    public List<string> Characters;
    public List<string> CharactersSkins;

    #endregion

    #region Menu Data's

    public bool SoundsIsMute;
    public float SoundsVolume;

    #endregion
}

public class _Cache_Save_System_ : MonoBehaviour
{
    public AllUserData UserData=new AllUserData();
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

    private void Awake()
    {
        LoadData();
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
                Debug.Log("UserLocalData loaded from: " + filePath);
                UserData= data;
                #region Sync to start!
                GameObject.FindObjectOfType<User_Interface>()._ImageChange();
                GameObject.FindObjectOfType<User_Interface>().ScroolVolumeSound.value = UserData.SoundsVolume;
                #endregion
            }
            else
            {

                SaveData();
                Debug.Log("Save file-'UserLocalData' not found and Saved new file to path : " + filePath);
            }
        }catch(Exception ex) { Debug.Log(ex);}
    }
    private void OnApplicationQuit()
    {
        SaveData();
        Debug.Log("HasSaved!");
    }
}
