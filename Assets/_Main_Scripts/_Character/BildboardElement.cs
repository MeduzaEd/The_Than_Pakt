using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BildboardElement : MonoBehaviour
{
    private Transform _camera;
    private void Start()
    {
        _camera = Camera.current !=null? Camera.current.transform:null;
    }
    private void LateUpdate()
    {
        if (_camera==null) { try { _camera = Camera.current.transform; }catch{ return; } return; }
        transform.SetPositionAndRotation(transform.position, _camera.rotation);
    }
}
