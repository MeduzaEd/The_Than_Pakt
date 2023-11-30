using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class User_Control : NetworkBehaviour
{
    public Camera _camera;
    public Rigidbody _rb;
    public NetworkVariable<bool> UserLoaded;
    private Rigidbody _srb;
    private bool OnRotating = false;
    private float prevXRotation = 0f;
    public float currentZoomDistance = 0f;
    private Vector3 MovingTo= Vector3.zero;
    private Vector3 offset = Vector3.zero;
    public Vector3 _offset = new Vector3(0,0.5f,0);

    IEnumerator WaitFromLoadCharacter()
    {
        
        do
        {

            yield return new WaitForSecondsRealtime(0.125f);
            yield return null;
        } while (UserLoaded.Value==false);
        _rb = GetComponentInChildren<Rigidbody>();
        _camera = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Camera>();
        if (IsLocalPlayer)
        {
            _camera.enabled = true;
            _camera.GetComponent<AudioListener>().enabled = true;
            if(IsHost)
            {
                _rb.interpolation = RigidbodyInterpolation.None;
            }
        }
        yield return null;
    }
    public void Start ()
    {
        Debug.Log($"Player Started id:{IsLocalPlayer}");
        #region Init Allows and Server!!!

        StartCoroutine(WaitFromLoadCharacter());

        #endregion

        //if (!IsOwner || !IsClient) { return; }
     
    }
    [ServerRpc]
    private void MoveServerRpc(ulong uid ,Vector3 _MoveVector, Vector3 forwardDirection, Vector3 rightDirection)
    {
        if (_MoveVector == Vector3.zero||!IsServer ){ return; }
        _MoveVector.Normalize();
        forwardDirection.y = 0f;
        forwardDirection.Normalize();
        rightDirection.Normalize();
        NetworkObject User = NetworkManager.SpawnManager.GetPlayerNetworkObject(uid);
        // —оздаем вектор движени€, комбиниру€ направлени€ вправо и вперед
        Vector3 moveDirection = (rightDirection * _MoveVector.x) + (forwardDirection * _MoveVector.z);
        moveDirection = moveDirection.normalized;
        _srb = User.GetComponentInChildren<Rigidbody>();
        // ¬ычисл€ем и примен€ем окончательный вектор движени€
        Vector3 MoveVector = moveDirection * User.GetComponent<Humanoid>().Speed.Value;
        MoveVector = MoveVector * NetworkManager.ServerTime.FixedDeltaTime;
        _srb.velocity = MoveVector;

        // ѕоворачиваем персонаж в направлении движени€
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            _srb.transform.GetChild(1).transform.rotation = Quaternion.Slerp(_srb.transform.GetChild(1).transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
    private void MovingCamera(Vector3 rotationInput)
    {
        if (!IsLocalPlayer ) { return; }

        rotationInput = rotationInput.normalized;
        float _speed = 200f;
        if (rotationInput.magnitude >= 0.1f || Input.GetAxis("Mouse ScrollWheel")!=0)
        {

            //_camera.transform.parent.RotateAround(_rb.position, _camera.transform.parent.right, -rotationInput.z * _speed * Time.deltaTime);
            _camera.transform.parent.RotateAround(_rb.position, Vector3.up, rotationInput.x * _speed * Time.deltaTime);

            // ѕоворот вокруг оси X с ограничением углов

            float newRotationX = Mathf.Clamp(prevXRotation - rotationInput.z * _speed * Time.deltaTime, -40f, 55f);

            // ѕрименение поворота вокруг оси X с учетом ограничений
            _camera.transform.parent.rotation = Quaternion.Euler(newRotationX, _camera.transform.parent.rotation.eulerAngles.y, 0f);

            // ќбновление предыдущего угла
            prevXRotation = newRotationX;

            // Zoom using the scroll wheel
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            currentZoomDistance -= scrollInput * 1.1f;
            currentZoomDistance = Mathf.Clamp(currentZoomDistance, -0.1f, 1f);

            // Position the camera

        }
        offset = -_camera.transform.forward * currentZoomDistance + _offset;
        _camera.transform.parent.position = _rb.position + offset;
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
        if (!IsOwner|| UserLoaded.Value==false|| !_rb||!_camera) { return; }
        #region Moving
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Vector3 MovingTo = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            MoveServerRpc(OwnerClientId, MovingTo, _camera.transform.parent.forward, _camera.transform.parent.right);
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

        if (OnRotating == true && (Input.GetAxis("HorizontalRotation") != 0 || Input.GetAxis("VerticalRotation") != 0))
        {
            MovingTo = new Vector3(Input.GetAxis("HorizontalRotation"), 0, Input.GetAxis("VerticalRotation"));
        }
        else 
        {
            if (MovingTo != Vector3.zero)
            {
                MovingTo = Vector3.zero;
            }
        }

        MovingCamera(MovingTo);
        #endregion
    }
}
