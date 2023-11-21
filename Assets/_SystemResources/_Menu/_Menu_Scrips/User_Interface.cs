using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class User_Interface : MonoBehaviour
{
    
    public void AnimationSettingsChange(bool _To)
    {
        GetComponent<Animator>().SetBool("Settings", _To);
    }
    public void AnimationPlayMenuChange(bool _To)
    {
        GetComponent<Animator>().SetBool("Menu", _To);
    }
}
