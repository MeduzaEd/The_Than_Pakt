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
    private Vector3 Offset = new Vector3(-0.1f, 2f, -2f);
    public float MaxVerticalAngle = 80.0f; // Максимальный угол наклона вверх
    public float MinVerticalAngle = -80.0f; // Максимальный угол наклона вниз

    public float CameraZoom = 1f;
    public float CameraZoomMax = 2f;
    public float CameraSpeed = 5f;
    public float cameraspeed = 5f;
    [SerializeField]
    private float verticalRotation = 0.0f;


    [SyncVar]
    public bool InputLag = false;
    [SyncVar]
    public float PlayerSpeed=5f;
    private void Start()
    {
        RB = this.GetComponentInChildren<Rigidbody>();
        C = GameObject.FindObjectOfType<Camera>();
    }
    public void FixedUpdate()
    {

        if (!isLocalPlayer || !isClient) { return; }//is Not Server Check
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
        float horizontalInput = Input.GetAxis("HorizontalRotation"); // Получаем ввод для вращения
        float verticalInput = Input.GetAxis("VerticalRotation");
        Debug.Log($"H: {horizontalInput} |V:{verticalInput}");
        Vector3 Rbp = RB.transform.position;


        C.transform.RotateAround(Rbp, Vector3.up, horizontalInput * cameraspeed);
     
            // Вращение по вертикали с ограничением угла
            verticalRotation -= verticalInput * cameraspeed;
            verticalRotation = Mathf.Clamp(verticalRotation, MinVerticalAngle, MaxVerticalAngle);

            C.transform.rotation = Quaternion.Euler(verticalRotation, C.transform.eulerAngles.y, 0);
    
        Vector3 cameraTargetPosition = Rbp - C.transform.forward * Offset.z + Vector3.up * Offset.y;
        // Позиционирование камеры
        
        RaycastHit hit;
        if (Physics.Raycast(Rbp, cameraTargetPosition - Rbp, out hit, Vector3.Distance(Rbp, cameraTargetPosition)))
        {
            // Если луч пересекает что-либо, записываем позицию пересечения
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
        // Вызывайте метод движения на сервере
        if (!isServer) return;
        Vector3 Move = new Vector3(x, -.1f, z).normalized;

        Transform Cam = GameObject.FindObjectOfType<Camera>().transform;
        Cam.position = RB.position;
        Cam.rotation = Quaternion.Euler( 0,Rotation.eulerAngles.y ,0);
        
        Vector3 MoveNormal = ((Move.x * Cam.transform.right) +(Move.z* Cam.transform.forward) +((Move.y * Cam.transform.up)))* PlayerSpeed * Time.fixedDeltaTime;
        RpcDebuge(MoveNormal.ToString());
        RB.rotation =Quaternion.Euler( 0,(Quaternion.LookRotation(MoveNormal*10)).eulerAngles.y,0);
        RB.velocity=(MoveNormal);
        
        //ServerMoveOn(x, z);
        //else { Debug.Log("isNotServer"); }

    }
    [ClientRpc]
    public void RpcDebuge(string text)
    {
        Debug.Log($"isGlobal:{text},");
    }
    public void MoveOn(float x, float z)
    {
        if (InputLag==true)
        {
            Vector3 Move = new Vector3(x, -.1f, z).normalized;
            Vector3 MoveNormal = Move * PlayerSpeed * Time.fixedDeltaTime;
            RpcDebuge(MoveNormal.ToString());
            RB.velocity=( MoveNormal);
        }
        CmdMoveOn(x, z,C.transform.rotation);//Basic Move

    }
    #endregion

}
