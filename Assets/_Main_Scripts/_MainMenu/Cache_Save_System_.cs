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
        Debug.Log("HasSaved");
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        _SaveSingeton = this;
        LoadData();
        CheckToLoad();
        CheckToSelected();
        StartCoroutine(AutoSave());
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
                string encryptedJson = File.ReadAllText(filePath);

                // �������� ������� ������ � �����
                if (string.IsNullOrEmpty(encryptedJson))
                {
                    AllUserData defaultData = new();
                    SaveDefaultData(filePath, defaultData);
                    Debug.LogError("���� 'IsUserLocalData.json' ������ ��� ���������.");
                    return;
                }

                string decryptedJson = EncryptionManager.Decrypt(encryptedJson);

                // �������� ������� ������ ����� ������������
                if (string.IsNullOrEmpty(decryptedJson))
                {
                    AllUserData defaultData = new();
                    SaveDefaultData(filePath, defaultData);
                    Debug.LogError("���� 'IsUserLocalData.json' �������� ������������ ������.");
                    return;
                }

                AllUserData data = JsonUtility.FromJson<AllUserData>(decryptedJson);
                UserData = data;

                #region Sync to start!
                GameObject.FindObjectOfType<User_Interface>().ScroolVolumeSound.value = UserData.SoundsVolume;

                GameObject.FindObjectOfType<User_Interface>().ImageChange();
                GameObject.FindObjectOfType<User_Interface>().ScroolVolumeMaxConnections.value = ((float)UserData.MaxUsersInHost) / 16;
                GameObject.FindObjectOfType<User_Interface>().MaxUsersChange();
                GameObject.FindObjectOfType<User_Interface>().ScroolVolumeMaxConnections.onValueChanged.AddListener(The_MaxUsersChange);
                GameObject.FindObjectOfType<User_Interface>().ServerName.transform.parent.GetComponent<InputField>().text = UserData.MyServerName;
                GameObject.FindObjectOfType<User_Interface>().ServerNameChange(UserData.MyServerName);
                #endregion

                Debug.Log("UserLocalData loaded from: " + UserData.MaxUsersInHost);
            }
            else
            {
                AllUserData defaultData = new();
                SaveDefaultData(filePath, defaultData);
                Debug.Log("���� 'IsUserLocalData.json' �� ������. ������ ����� ���� �� ����: " + filePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"��������� ������ ��� �������� ������: {ex.Message}\nStackTrace: {ex.StackTrace}");
        }
    }
    private void SaveDefaultData(string filePath, AllUserData defaultData)
    {
        // ��������� ����� ������ �� ��������� � ����
        string defaultJson = JsonUtility.ToJson(defaultData);
        string encryptedJson = EncryptionManager.Encrypt(defaultJson);

        File.WriteAllText(filePath, encryptedJson);

        Debug.Log("������ ����� ���� 'IsUserLocalData.json' � ������� �� ���������.");
    }
    private void The_MaxUsersChange(float _)
    {
        GameObject.FindObjectOfType<User_Interface>().MaxUsersChange();
    }

    private void OnApplicationQuit()
    {
        SaveData();
        Debug.Log($" - HasSaved!");
    }
    IEnumerator AutoSave()
    {
        do
        {

            yield return new WaitForSecondsRealtime(9f);
            SaveData();
            yield return null;
        }
        while (true) ;
    }
}
