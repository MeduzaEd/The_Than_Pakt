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
        float _speed = 150f;
        rotationInput = rotationInput.normalized * Time.fixedDeltaTime* _speed;
    
        if (rotationInput.magnitude >= 0.1f || Input.GetAxis("Mouse ScrollWheel")!=0)
        {

            //_camera.transform.parent.RotateAround(_rb.position, _camera.transform.parent.right, -rotationInput.z * _speed * Time.deltaTime);
            _camera.transform.parent.RotateAround(_rb.position, Vector3.up, rotationInput.x );

            // ѕоворот вокруг оси X с ограничением углов

            float newRotationX = Mathf.Clamp(prevXRotation - rotationInput.z, -22.5f, 45f);
           // Debug.Log($"newRotationX:{newRotationX} & rotationInput:{rotationInput}");
            // ѕрименение поворота вокруг оси X с учетом ограничений
            _camera.transform.parent.rotation = Quaternion.Euler(newRotationX , _camera.transform.parent.rotation.eulerAngles.y, 0f);

            // ќбновление предыдущего угла
            prevXRotation = newRotationX;

            // Zoom using the scroll wheel
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            currentZoomDistance -= scrollInput * 400f*Time.deltaTime;
            currentZoomDistance = Mathf.Clamp(currentZoomDistance, -0.1f, 1.5f);

            // Position the camera

        }
        offset = ( - _camera.transform.forward * currentZoomDistance + _offset);
       
        //Debug.Log($"offeset:{offset} rb:{_rb.GetComponent<Transform>().position}|{_rb.transform.position} camera:{ _camera.transform.parent.position} |Rb+_offset:{_rb.transform.position + _offset} mag:{Vector3.Distance(_rb.position + _offset, _rb.position + _offset + offset)}");


      //  Debug.DrawRay(_rb.transform.position + _offset, offset - _offset, Color.red, 0.1f);
      //  Debug.DrawRay(_camera.transform.parent.position, (_camera.transform.right * 0.25f), Color.yellow, 0.1f);
       // Debug.DrawRay(_camera.transform.parent.position, (_camera.transform.right * -0.25f), Color.grey, 0.1f);
        RaycastHit hit;
        if (Physics.Raycast(_rb.transform.position + _offset, offset - _offset, out hit, Vector3.Distance(_rb.position + _offset, _rb.position + _offset+ offset), ~LayerMask.GetMask("CameraTransparent")))
        {
            RaycastHit hit2;
            _camera.transform.parent.position =hit.point + (_camera.transform.forward * .0125f);
     
            if (Physics.Raycast(_camera.transform.parent.position,_camera.transform.right * .0125f, out hit2, Vector3.Distance(_camera.transform.parent.position, _camera.transform.right * .0125f), ~LayerMask.GetMask("CameraTransparent")))
            {
            //    Debug.Log("Hit2");
                _camera.transform.parent.position = hit2.point + (_camera.transform.right * -.0125f);
            }
            else if (Physics.Raycast(_camera.transform.parent.position, (_camera.transform.right * -.0125f), out hit2, Vector3.Distance(_camera.transform.parent.position,(_camera.transform.right * -.0125f)), ~LayerMask.GetMask("CameraTransparent")))
            {
             //   Debug.Log("Hit3");
                _camera.transform.parent.position = hit2.point - (_camera.transform.right * -.0125f);
            }

          //  Debug.Log("Hit");
        }else
        {
            _camera.transform.parent.position = _rb.transform.position + offset;
        }
        
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
