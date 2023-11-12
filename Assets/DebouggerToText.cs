using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public  class DebouggerToText : MonoBehaviour
{

    private static Text outdebug;
    public static void DEBUGLOG(string info)
    {
        if(outdebug==null){ return; }
        outdebug.text = info;
    }
    private void Start()
    {
        outdebug = GetComponent<Text>();
    }
}
