using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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
    public bool TutorialSkip;
    public bool SoundsIsMute;
    public float SoundsVolume;
    public int MaxUsersInHost;
    public string MyServerName;
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

        // ������� ����������, ���� �� ���
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // ���������, ���������� �� ����
        if (File.Exists(filePath))
        {
            // ���� ����������, ��������� ���
            File.WriteAllText(filePath, encryptedJson);
            Debug.Log("Data updated at: " + filePath);
        }
        else
        {
            // ���� ����� ���, ������� �����
            File.WriteAllText(filePath, encryptedJson);
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
                GameObject.FindObjectOfType<User_Interface>()._ImageChange();
                GameObject.FindObjectOfType<User_Interface>().ScroolVolumeMaxConnections.value =((float)UserData.MaxUsersInHost)/16;
                GameObject.FindObjectOfType<User_Interface>()._TextChangeInMaxUsers();
                GameObject.FindObjectOfType<User_Interface>().ScroolVolumeMaxConnections.onValueChanged.AddListener(The_MaxUsersChange);
                GameObject.FindObjectOfType<User_Interface>().ServerName.transform.parent.GetComponent<InputField>().text = UserData.MyServerName ;
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
        GameObject.FindObjectOfType<User_Interface>()._MaxUsersChange();
    }
    private void OnApplicationQuit()
    {
        SaveData();
        Debug.Log($"{ UserData.MaxUsersInHost} - HasSaved!");
    }
}
