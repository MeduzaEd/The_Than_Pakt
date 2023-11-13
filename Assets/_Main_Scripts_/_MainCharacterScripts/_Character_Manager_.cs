using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

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
    private float ZoomInput = 0.1f;
    private float MaxVerticalAngle = 66.0f; // Максимальный угол наклона вверх
    private float MinVerticalAngle = -33.0f; // Максимальный угол наклона вниз
    // private Object and Script Variables 
    //private _humanoid_ humanoid;
    private _player_ player;
    private GameObject character;
    private Transform _camera;
    private Rigidbody rb;
    private FixedJoystick fixedJoystick;
    // Private Vectors vat
    private Vector2 notochscreenposition = new Vector2(Screen.width / 3, Screen.height / 3);
    public GameObject FindObjectByNetID(uint netID)
    {
        if (NetworkServer.spawned.TryGetValue(netID, out NetworkIdentity obj))
        {
            return obj.gameObject;
        }
        return null;
    }
    private void Start()
    {
        //if (!isLocalPlayer) return;
        //humanoid = transform.parent.GetComponent<_humanoid_>();
        //Camera
        _camera = GameObject.FindObjectOfType<Camera>().transform;
        //Joystick 
        fixedJoystick = GameObject.FindObjectOfType<FixedJoystick>();
        //Player
        player = this.transform.parent.parent.GetComponent<_player_>();
        //Rigid Body
        rb = GetComponent<Rigidbody>();
        //Spawn
        character = Instantiate(Resources.Load<GameObject>(player.Character_Path));
        GameObject Skin = Instantiate(Resources.Load<GameObject>(player.Skin_Path));
        player.gameObject.name = player.GetComponent<NetworkIdentity>().netId.ToString();
        Skin.transform.SetParent(character.transform);
        character.transform.SetParent(this.transform);
        character.transform.localPosition = Vector3.zero;
        character.name = character.GetComponent<NetworkIdentity>().netId.ToString();

        //Spawn On Network

        Debug.Log("ONSPAWN");
        CharacterSpawn(player.GetComponent<NetworkIdentity>().netId);
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer) { return; }//Break On Not Local Player

        CharacterUpdate();
        _CameraUpdate();
    }
    [Command(requiresAuthority = false)]
    private void CharacterSpawn(uint NetworkID)
    {
        if (!isServer|| FindObjectByNetID(NetworkID) == null) return;
        GameObject player = FindObjectByNetID(NetworkID);
        NetworkServer.Spawn(character, player);
        
    }

    [Command(requiresAuthority = false)]
    private void CharacterMove(float H, float V, uint NetworkID, float cruay)
    {

        if (!isServer || FindObjectByNetID(NetworkID) == null) return;
        if (H == 0 && V == 0) return;
        //Debug.Log($"themove {NetworkID}");
        Variables variables = FindObjectByNetID(NetworkID).transform.GetChild(0).GetComponent<_humanoid_>().variables;
        GameObject LocalCam = FindObjectByNetID(NetworkID).transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        if (variables.Died == true || variables.Stun == true|| variables.Stopped == true|| variables.Pushed == true) { return; }
       // Debug.Log("onthemove");
        LocalCam.transform.position = rb.position;
        LocalCam.transform.rotation = Quaternion.Euler(0,cruay, 0);
        Vector3 Move = new Vector3(H, -.125f, V).normalized;
        Vector3 MoveNormal = ((Move.x * LocalCam.transform.right) + (Move.z * LocalCam.transform.forward)+(Move.y * LocalCam.transform.up)) * 125f * Time.fixedDeltaTime;
        rb.rotation = Quaternion.Euler(0, (Quaternion.LookRotation(MoveNormal * 10)).eulerAngles.y, 0);
        rb.velocity=MoveNormal;
    }
    private void CharacterUpdate()
    {
        if (fixedJoystick != null)
        {
            if (fixedJoystick.Horizontal != 0 || fixedJoystick.Vertical != 0)
            {
                // Debug.Log("move");
                CharacterMove(fixedJoystick.Horizontal, fixedJoystick.Vertical, player.GetComponent<NetworkIdentity>().netId, _camera.rotation.eulerAngles.y);
                return;
            }
        }
        if (Input.GetAxis("Horizontal") !=0|| Input.GetAxis("Vertical") != 0)
        {
           // Debug.Log("onmove");
            CharacterMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") , player.GetComponent<NetworkIdentity>().netId, _camera.rotation.eulerAngles.y);
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
                if(Input.GetAxis("Horizontal")!=0 || Input.GetAxis("Vertical") != 0 || fixedJoystick.Horizontal != 0 || fixedJoystick.Vertical != 0) { return; }//Break On Move
                // MOBILE 
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                //  текущее расстояние между пальцами
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);

                //  начальное расстояние между пальцами
                float startDistance = Vector2.Distance(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);

                //  изменение масштаба
                float zoomAmount = (startDistance - currentDistance) * -CameraZoomSpeed;

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

        // if (ZoomInput !=0 && fixedJoystick.Horizontal == 0 && fixedJoystick.Vertical == 0)
        //  {horizontalInput = 0f;verticalInput = 0f;}
      //  try
   //     {
            if (Input.GetMouseButton(1)) { realcameraspeed = CameraSpeed; } else { realcameraspeed = 0; }

            
        if (Input.touchCount > 0)
        {
            Touch touch1 = Input.GetTouch(0);
            if ((notochscreenposition.x < touch1.position.x && notochscreenposition.y < touch1.position.y))
            {
                horizontalInput = touch1.deltaPosition.x;
                verticalInput = touch1.deltaPosition.y;
                realcameraspeed = CameraSpeed;
            }

            if (Input.touchCount >= 2)
            {
                Touch touch2 = Input.GetTouch(1);
                if ((notochscreenposition.x < touch2.position.x && notochscreenposition.y < touch2.position.y))
                {
                    horizontalInput = touch2.deltaPosition.x;
                    verticalInput = touch2.deltaPosition.y;
                    realcameraspeed = CameraSpeed;
                }
            }
            DebouggerToText.DEBUGLOG($"horizontal :{horizontalInput} vertical:{verticalInput} 0 touchpos:{Input.GetTouch(0).position}");
        }
      //  }
       // catch (Exception ex) { Debug.Log(ex); horizontalInput = 0;verticalInput = 0; }
        _camera.transform.RotateAround(rb.position, Vector3.up, horizontalInput * realcameraspeed);
        verticalRotation -= verticalInput * realcameraspeed;
        verticalRotation = Mathf.Clamp(verticalRotation, MinVerticalAngle, MaxVerticalAngle);
        _camera.transform.rotation = Quaternion.Slerp(_camera.transform.rotation, Quaternion.Euler(verticalRotation, _camera.transform.eulerAngles.y, 0), Time.deltaTime * CameraSlerpSpeed);
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
