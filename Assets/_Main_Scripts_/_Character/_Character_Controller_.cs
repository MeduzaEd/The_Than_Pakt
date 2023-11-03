using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class _Character_Controller_ : NetworkBehaviour
{
    [SerializeField]
    private Camera C;
    [SerializeField]
    private Rigidbody RB;


    [SerializeField]
    Animator _Animator;
    [SerializeField]
    private Vector3 Offset = new Vector3(-0.1f, 2f, -2f);
    public float MaxVerticalAngle = 80.0f; // ������������ ���� ������� �����
    public float MinVerticalAngle = -80.0f; // ������������ ���� ������� ����

    public float CameraZoom = 1f;
    public float CameraZoomMax = 2f;
    public float CameraSpeed = 5f;
    public float cameraspeed = 5f;
    [SerializeField]
    private float verticalRotation = 0.0f;
    private Humanoid hum;

    [SyncVar]
    public bool InputLag = false;
    private void Start()
    {
        hum = this.GetComponentInChildren<Humanoid>();
        RB = this.GetComponentInChildren<Rigidbody>();
        C = GameObject.FindObjectOfType<Camera>();
    }
    void Update()
    {
        if (!isLocalPlayer || !isClient) { return; }
        float ZoomInput = Input.GetAxis("Mouse ScrollWheel");
        Debug.Log(ZoomInput);
    }
    public void FixedUpdate()
    {
      
        if (!isLocalPlayer || !isClient) { return; }//is Not Server Check
        _Animator.SetFloat("Speed", RB.velocity.magnitude);
        if (Input.GetAxis("Horizontal") != 0 || 0 != Input.GetAxis("Vertical"))
        {
            if (!RB) { return; }

                float horizontalInput = Input.GetAxis("Horizontal");// X
                float verticalInput = Input.GetAxis("Vertical");// Z
                
                MoveOn(horizontalInput, verticalInput);
               // Debug.Log("isLocalMove: OnStart");
        }

    }
    public void LateUpdate()
    {
        if (!isLocalPlayer || !isClient) { return; }//is Not Server Check
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (Input.GetMouseButton(0)) { cameraspeed = CameraSpeed; } else { cameraspeed = 0; }
        }
        else
        {
            if (Input.GetMouseButton(1)) { cameraspeed = CameraSpeed; } else { cameraspeed = 0; }
        }
        float horizontalInput = Input.GetAxis("HorizontalRotation"); // �������� ���� ��� ��������
        float verticalInput = Input.GetAxis("VerticalRotation");
       
        Vector3 Rbp = RB.transform.position;
       

        C.transform.RotateAround(Rbp, Vector3.up, horizontalInput * cameraspeed);
     
            // �������� �� ��������� � ������������ ����
            verticalRotation -= verticalInput * cameraspeed;
            verticalRotation = Mathf.Clamp(verticalRotation, MinVerticalAngle, MaxVerticalAngle);

            C.transform.rotation = Quaternion.Euler(verticalRotation, C.transform.eulerAngles.y, 0);
    
        Vector3 cameraTargetPosition = Rbp - C.transform.forward * Offset.z + Vector3.up * Offset.y;
        // ���������������� ������
        
        RaycastHit hit;
        if (Physics.Raycast(Rbp, cameraTargetPosition - Rbp, out hit, Vector3.Distance(Rbp, cameraTargetPosition)))
        {
            // ���� ��� ���������� ���-����, ���������� ������� �����������
            Vector3 newPosition = hit.point;
            C.transform.position = newPosition;
        }
        else
        {
            C.transform.position = cameraTargetPosition;
        }
    }
    #region Move
    [Command]
    public void CmdMoveOn(float x, float z,Quaternion Rotation)
    {
        
        // ��������� ����� �������� �� �������
        if (!isServer) return;
        Vector3 Move = new Vector3(x, -.1f, z).normalized;

        Transform Cam = GameObject.FindObjectOfType<Camera>().transform;
        Cam.position = RB.position;
        Cam.rotation = Quaternion.Euler( 0,Rotation.eulerAngles.y ,0);
       
        Vector3 MoveNormal = ((Move.x * Cam.transform.right) +(Move.z* Cam.transform.forward) +((Move.y * Cam.transform.up)))* hum.Speed * Time.fixedDeltaTime;
        RB.rotation =Quaternion.Euler( 0,(Quaternion.LookRotation(MoveNormal*10)).eulerAngles.y,0);
        RB.velocity=(MoveNormal);
        

    }

    public void MoveOn(float x, float z)
    {
        
        Debug.Log(RB.velocity.magnitude);
        CmdMoveOn(x, z,C.transform.rotation);//Basic Move
        
    }
    #endregion

}
