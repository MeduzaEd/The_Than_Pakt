using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;
public class _Character_Manager_ : NetworkBehaviour
{
    //private Vector var
    [SerializeField]
    private Transform LocalCameraPosition;
    [SerializeField]
    private Vector3 Offset = new Vector3(-0.1f, 1f, -2f);
    //Public float var 
    public float CameraZoomSpeed = 0.25f;
    public float CameraZoomMin = 1.2f;
    public float CameraZoomMax = 9f;
    public float CameraSpeed = 5f;
    public float CameraSlerpSpeed = 3f;
    public float realcameraspeed = 5f;
    //Private float var 
    private float verticalRotation = 0.0f;
    private float ZoomInput = 0;
    private float MaxVerticalAngle = 70.0f; // Максимальный угол наклона вверх
    private float MinVerticalAngle = -35.0f; // Максимальный угол наклона вниз
    // private Object and Script Variables 
    private _player_ player;
    private GameObject character;
    private Transform _camera;
    private Rigidbody rb;
    private FixedJoystick fixedJoystick;

    private void Start()
    {
        //Camera
        _camera = GameObject.FindObjectOfType<Camera>().transform;
        //Joystick 
        fixedJoystick = GameObject.FindObjectOfType<FixedJoystick>();
        //Player
        player = this.transform.parent.parent.GetComponent<_player_>();
        //Rigid Body
        rb = GetComponent<Rigidbody>();
        //Spawn

        character =Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(player.Character_Path));
        GameObject Skin = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(player.Skin_Path));
        
        Skin.transform.SetParent(character.transform);
        character.transform.SetParent(this.transform);
       
        //Spawn On Network
        CharacterSpawn();
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }//Break On Not Local Player

        CharacterUpdate();
        _CameraUpdate();
    }
    [Command]
    private void CharacterSpawn()
    {
        NetworkServer.Spawn(character);
        character.transform.localPosition = Vector3.zero;
    }

    [Command]
    private void CharacterMove(float H, float V)
    {

        if (!isServer || _camera==null|| LocalCameraPosition==null) return;
        if (H == 0 && V == 0) return;
        
        LocalCameraPosition.position = rb.position;
        LocalCameraPosition.rotation = Quaternion.Euler(0, _camera.rotation.eulerAngles.y, 0);
        Vector3 Move = new Vector3(H, -.125f, V).normalized;
        Vector3 MoveNormal = ((Move.x * LocalCameraPosition.transform.right) + (Move.z * LocalCameraPosition.transform.forward)+(Move.y * LocalCameraPosition.transform.up)) * 125f * Time.fixedDeltaTime;
        rb.rotation = Quaternion.Euler(0, (Quaternion.LookRotation(MoveNormal * 10)).eulerAngles.y, 0);
        rb.velocity=MoveNormal;

    }
    private void CharacterUpdate()
    {
        if(fixedJoystick.Horizontal !=0 || fixedJoystick.Vertical!=0)
        {
            CharacterMove(fixedJoystick.Horizontal, fixedJoystick.Vertical);
            return;
        }
        if (Input.GetAxis("Horizontal") !=0|| Input.GetAxis("Vertical") != 0)
        {
            CharacterMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") );
            return;
        }
    }
    private void _CameraUpdate()
    {
        _CameraRotation();
        _CameraScroll();
        if (ZoomInput !=0)
        {
            Offset = Vector3.Lerp(Offset, new Vector3(-0.1f, Offset.y , Mathf.Clamp((Offset.z) + ZoomInput, -CameraZoomMax, -CameraZoomMin)), Time.deltaTime * 125f);
            ZoomInput = 0;
        }
    }
    private void _CameraScroll()
    {
        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            ZoomInput = Input.GetAxis("Mouse ScrollWheel") * ((CameraZoomSpeed) * 25f);
        }
        else
        {

            if (Input.touchCount == 2 )
            {
                if(Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical") != 0) { return; }//Break On Move
                // MOBILE 
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                //  текущее расстояние между пальцами
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);

                //  начальное расстояние между пальцами
                float startDistance = Vector2.Distance(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);

                //  изменение масштаба
                float zoomAmount = (startDistance - currentDistance) * CameraZoomSpeed;

                //  изменение масштаба с ограничениями
                ZoomInput = zoomAmount;
                Debug.Log(zoomAmount);
            }
        }
    }
    private void _CameraRotation()
    {
        float horizontalInput = Input.GetAxis("HorizontalRotation");
        float verticalInput = Input.GetAxis("VerticalRotation");
        if (Input.touchCount == 2 && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {horizontalInput = 0f;verticalInput = 0f;}
        if (Input.GetMouseButton(1)) { realcameraspeed = CameraSpeed; } else { realcameraspeed = 0; }
        _camera.transform.RotateAround(rb.position, Vector3.up, horizontalInput * realcameraspeed);
        verticalRotation -= verticalInput * realcameraspeed;
        verticalRotation = Mathf.Clamp(verticalRotation, MinVerticalAngle, MaxVerticalAngle);
        _camera.transform.rotation =Quaternion.Slerp(_camera.transform.rotation, Quaternion.Euler(verticalRotation, _camera.transform.eulerAngles.y, 0),Time.deltaTime* CameraSlerpSpeed);
        Vector3 cameraTargetPosition = rb.position + _camera.transform.forward * Offset.z + Vector3.up * Offset.y;
        RaycastHit hit;
        if (Physics.Raycast(rb.position, cameraTargetPosition - rb.position, out hit, Vector3.Distance(rb.position, cameraTargetPosition)))
        {
            _camera.transform.position = hit.point;
            
        }
        else
        {
           // _camera.transform.position = cameraTargetPosition;
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, cameraTargetPosition, Time.deltaTime * CameraSlerpSpeed);

        }

    }
}
