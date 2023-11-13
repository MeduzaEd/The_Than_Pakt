using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Windows.Input;
public class GameUI_Manager : MonoBehaviour
{
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(SystemInfo.deviceType != DeviceType.Desktop);
        
    }
}
