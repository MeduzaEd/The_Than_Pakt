using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Character_Eldric : NetworkBehaviour
{
    private Camera _camera;
    private Rigidbody _rb;
    
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
        Vector3 MoveVector = new Vector3(_MoveVector.x, 0,_MoveVector.z).normalized ;
        MoveVector = (servercamera.right * _MoveVector.x)+( servercamera.forward * _MoveVector.z)+(servercamera.up*0f);
        MoveVector = MoveVector * NetworkManager.SpawnManager.GetPlayerNetworkObject(UserId).GetComponent<Humanoid>().Speed.Value;
        MoveVector = MoveVector * NetworkManager.ServerTime.FixedDeltaTime;
        serverrb.AddForce(MoveVector, ForceMode.Force);
    }
    private void Update()
    {
        if (!IsOwner || !IsClient|| (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)) { return; }
        Vector3 MovingTo = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MoveServerRpc(OwnerClientId, MovingTo);

    }
}
