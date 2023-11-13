using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
public class GameUI_Manager : MonoBehaviour
{
    public NetworkManager networkManager;
    private void Start()
    {
        networkManager = GameObject.FindObjectOfType<NetworkManager>();
        transform.GetChild(0).gameObject.SetActive(SystemInfo.deviceType != DeviceType.Desktop);
    }
    public void LeaveToMainMenu()
    {
        if (NetworkClient.isConnected&&networkManager!=null)
        {
            //networkManager.StopClient();
            Disconnect();
        }
        
    }
    public void Disconnect()
    {
        if (NetworkServer.activeHost)
        {
            networkManager.StopHost();

        }
        else if (NetworkServer.active)
        {
            networkManager.StopServer();
        }
        else if (NetworkClient.isConnected)
        {
            networkManager.StopClient();
        }
    }
}
