using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Character_Eldric : NetworkBehaviour
{
    private Camera _camera;
    private Rigidbody _rb;
    private bool OnRotating = false;
    private void Start()
    {
        if (!IsOwner || !IsClient) { return; }
        _camera = GetComponentInChildren<Camera>();
        _rb = GetComponent<Rigidbody>();
    }
    [ServerRpc]
    private void MoveServerRpc(ulong UserId,Vector3 _MoveVector)
    {
        if (_MoveVector == Vector3.zero||!IsServer|| NetworkManager.SpawnManager.GetPlayerNetworkObject(UserId)==null) { return; }
        Rigidbody serverrb = NetworkManager.SpawnManager.GetPlayerNetworkObject(UserId).GetComponentInChildren<Rigidbody>();
        Transform servercamera = serverrb.GetComponentInChildren<Camera>().transform;
        Vector3 MoveVector = (servercamera.right * _MoveVector.x)+( servercamera.forward * _MoveVector.z)+(servercamera.up*0f).normalized;
        MoveVector = MoveVector * NetworkManager.SpawnManager.GetPlayerNetworkObject(UserId).GetComponent<Humanoid>().Speed.Value;
        MoveVector = MoveVector * NetworkManager.ServerTime.FixedDeltaTime;
        serverrb.velocity=MoveVector;
    }
    [ServerRpc]
    private void RotateServerRpc(Vector3 rotationInput)
    {
        if (rotationInput == Vector3.zero || !IsServer ) { return; }
        Transform target = transform.GetComponentInChildren<Transform>();
        Transform serverCamera = target.GetChild(0);
        serverCamera.Rotate(Vector3.left, rotationInput.z * 3, Space.World);
        serverCamera.Rotate(Vector3.up, rotationInput.x * 3, Space.World);
        serverCamera.transform.rotation = Quaternion.Euler(serverCamera.rotation.x,serverCamera.rotation.y,0);
        //servercamera.transform.rotation = Quaternion.LookRotation(Target.position);
        //servercamera.RotateAround(Target.position, Vector3.up, _MoveVector.x*2);
        //Vector3 cameraTargetPosition = rb.position + _camera.transform.forward * Offset.z + Vector3.up * Offset.y;
        //RaycastHit hit;
        //if (Physics.Raycast(rb.position, cameraTargetPosition - rb.position, out hit, Vector3.Distance(rb.position, cameraTargetPosition), ~LayerMask.NameToLayer("CameraTransparent")))
        //{
        //     _camera.transform.position = hit.point;

        //}
    }
    private void Update()
    {
        if (!IsOwner) { return; }
        #region Moving
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Vector3 MovingTo = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            MoveServerRpc(OwnerClientId, MovingTo);
        }
        #endregion

        #region CameraUpdate
        if (Input.GetMouseButton(1))
        {
            OnRotating = true;
        }
        else 
        {
            OnRotating = false; 
        }

        if (OnRotating ==true &&(Input.GetAxis("HorizontalRotation") != 0 || Input.GetAxis("VerticalRotation") != 0))
        {
            Vector3 MovingTo = new Vector3(Input.GetAxis("HorizontalRotation"), 0, Input.GetAxis("VerticalRotation"));
            RotateServerRpc(MovingTo);
        }
        #endregion
    }
}
