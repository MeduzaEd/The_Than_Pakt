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
        string json = JsonUtility.ToJson(UserData);
        string directoryPath = Path.Combine(Application.persistentDataPath, "MeduzaEdCompany", "Soul_Night");
        string filePath = Path.Combine(directoryPath, "UserLocalData.json");

        // Создаем директорию, если ее нет
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Проверяем, существует ли файл
        if (File.Exists(filePath))
        {
            // Если существует, обновляем его
            File.WriteAllText(filePath, json);
            Debug.Log("Data updated at: " + filePath);
        }
        else
        {
            // Если файла нет, создаем новый
            File.WriteAllText(filePath, json);
            Debug.Log("Data saved at: " + filePath);
        }
    }

    private void Start()
    {
        LoadData();
    }

    public void LoadData()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "MeduzaEdCompany", "Soul_Night");
        string filePath = Path.Combine(directoryPath, "UserLocalData.json");
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                AllUserData data = JsonUtility.FromJson<AllUserData>(json);

                Debug.Log("UserLocalData loaded from: " + filePath);
                UserData= data;
            }
            else
            {
                Directory.CreateDirectory(directoryPath);
                Debug.LogError("Save file-'UserLocalData' not found at : " + filePath);
            }
        }catch(Exception ex) { Debug.Log(ex);}
    }
}
