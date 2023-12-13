using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUILookToCamera : MonoBehaviour
{
    Transform _LocalCamera; 
    void Start()
    {
        _LocalCamera = Camera.current.transform.parent.transform;
    }
    void LateUpdate()
    {
        if(_LocalCamera!=null)
        {
            transform.rotation = _LocalCamera.rotation;
        }
    }
}
