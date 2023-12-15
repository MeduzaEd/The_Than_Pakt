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
    public float currentZoomDistance = 0.7f;
    private Vector3 MovingTo= Vector3.zero;
    private Vector3 offset = Vector3.zero;
    public Vector3 _offset = new(0,0.5f,0);
    private Animator SkinAnimator;
    private readonly float _Rotatespeed = 100f;
    #region Coroutine
    public void Start()
    {
        
        #region Init Allows and Server!!!
        StartCoroutine(WaitFromLoadCharacter());
        #endregion
    }
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
            OnStartServerRpc(OwnerClientId);
            
        }
        yield return null;
    }
    #endregion
    #region ServerRpc
    [ServerRpc]
    private void OnStartServerRpc(ulong _ID)
    {
        if ((!IsServer)) { return; }
        
        _rb = NetworkManager.SpawnManager.GetPlayerNetworkObject(_ID).transform.GetChild(0).GetComponent<Rigidbody>();
        _rb.AddForce(Vector3.up*-5,ForceMode.Impulse);
    }


    [ServerRpc]
    private void MoveServerRpc(ulong uid ,Vector3 _MoveVector, Vector3 forwardDirection, Vector3 rightDirection)
    {
        
        if (!IsServer ){ return; }
        NetworkObject User = NetworkManager.SpawnManager.GetPlayerNetworkObject(uid);
        Humanoid _Humanoid = User.GetComponent<Humanoid>();
        _srb = User.GetComponentInChildren<Rigidbody>();
        SkinAnimator = _srb.transform.GetChild(2).GetChild(0).GetComponent<Animator>();
        SkinAnimator.SetFloat("Speed", _rb.velocity.magnitude);
        if (_Humanoid.OnAttack.Value == true) { return; }
        if (_MoveVector == Vector3.zero) { return; }
        _MoveVector.Normalize();
        forwardDirection.y = 0f;
        forwardDirection.Normalize();
        rightDirection.Normalize();
    
        // —оздаем вектор движени€, комбиниру€ направлени€ вправо и вперед
        Vector3 moveDirection = (rightDirection * _MoveVector.x) + (forwardDirection * _MoveVector.z);
        moveDirection = moveDirection.normalized;
      
        // ¬ычисл€ем и примен€ем окончательный вектор движени€
        Vector3 MoveVector =0.01f* _Humanoid.Speed.Value* _Humanoid.SpeedMulti.Value * moveDirection;
        MoveVector*= NetworkManager.ServerTime.FixedDeltaTime;
        _srb.velocity = MoveVector;

        // ѕоворачиваем персонаж в направлении движени€
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            _srb.transform.GetChild(2).transform.rotation = Quaternion.Slerp(_srb.transform.GetChild(2).transform.rotation, targetRotation, Time.deltaTime * 25f);
        }
    }
    #endregion

    #region Camera
    private void MovingCamera(Vector3 rotationInput)
    {
        if (!IsLocalPlayer ) { return; }
       
        rotationInput = Time.fixedDeltaTime * _Rotatespeed* rotationInput.normalized;
        if (rotationInput.magnitude >= 0.025f || Input.GetAxis("Mouse ScrollWheel")!=0)
        {

            _camera.transform.parent.RotateAround(_rb.position, Vector3.up, rotationInput.x );
            float newRotationX = Mathf.Clamp(prevXRotation - rotationInput.z, -25f, 45f);
            _camera.transform.parent.rotation = Quaternion.Euler(newRotationX , _camera.transform.parent.rotation.eulerAngles.y, 0f);
            prevXRotation = newRotationX;
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            currentZoomDistance -= scrollInput * 400f*Time.deltaTime;
            currentZoomDistance = Mathf.Clamp(currentZoomDistance, 0.4f, 1.4f);
        }
        offset = ( - _camera.transform.forward * currentZoomDistance) + _offset;

        if (Physics.Raycast(_rb.transform.position + _offset, offset - _offset, out RaycastHit hit, Vector3.Distance(_rb.transform.position + _offset, _rb.transform.position + offset ), ~LayerMask.GetMask("CameraTransparent")))
        {
            
            _camera.transform.parent.position = hit.point;
            //_camera.transform.parent.position = _camera.transform.parent.transform.forward * 1.0125f;
            Debug.DrawRay(_rb.transform.position + _offset, offset - _offset, Color.red, 0.1f);
        }
        else
        {
            Debug.DrawRay(_rb.transform.position + _offset, offset - _offset, Color.blue, 0.1f);
            _camera.transform.parent.position = _rb.transform.position + offset;
        }


       // Debug.DrawRay(_camera.transform.parent.position, (_camera.transform.right * -0.25f), Color.yellow, 0.1f);
       // Debug.DrawRay(_camera.transform.parent.position, (_camera.transform.right * 0.25f), Color.yellow, 0.1f);
    }
    #endregion

    #region Update
    private void Update()
    {
        
        if (!IsOwner|| UserLoaded.Value==false|| !_rb||!_camera) { return; }
        #region Moving
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Vector3 MovingTo = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            MoveServerRpc(OwnerClientId, MovingTo, _camera.transform.parent.forward, _camera.transform.parent.right);
        }else
        {
            MoveServerRpc(OwnerClientId, Vector3.zero, _camera.transform.parent.forward, _camera.transform.parent.right);
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
    #endregion 
}
