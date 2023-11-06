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
    private Vector3 Offset = new Vector3(-0.1f, 1f, -2f);
    public float MaxVerticalAngle = 80.0f; // Максимальный угол наклона вверх
    public float MinVerticalAngle = -80.0f; // Максимальный угол наклона вниз

    public float CameraZoomSpeed = 0.25f;
    public float CameraZoomMin = 1f;
    public float CameraZoomMax = 2f;
    public float CameraSpeed = 5f;
    public float cameraspeed = 5f;
    [SerializeField]
    private float verticalRotation = 0.0f;
    private Humanoid hum;
    private Vector2 touchStart;
    private FixedJoystick FJ;
    float ZoomInput = 0;
    [SyncVar]
    public bool InputLag = false;
    private void Start()
    {
        hum = this.GetComponentInChildren<Humanoid>();
        RB = this.GetComponentInChildren<Rigidbody>();
        C = GameObject.FindObjectOfType<Camera>();
        FJ = GameObject.FindObjectOfType<FixedJoystick>();
        ZoomInput = -10f;
    }
    void Update()
    {
        if (!isLocalPlayer || !isClient) { return; }
        
        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
             ZoomInput = Input.GetAxis("Mouse ScrollWheel")* ((CameraZoomSpeed)*25f);
        }
        else
        {

            if (Input.touchCount == 2)
            {
                // MOBILE 
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                // Рассчитайте текущее расстояние между пальцами
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);

                // Рассчитайте начальное расстояние между пальцами
                float startDistance = Vector2.Distance(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);

                // Рассчитайте изменение масштаба
                float zoomAmount = (startDistance - currentDistance) * CameraZoomSpeed;

                // Примените изменение масштаба с ограничениями
                ZoomInput = zoomAmount;
                Debug.Log(zoomAmount);
            }
        }
        if (ZoomInput != 0)
        {
            Offset=Vector3.Lerp(Offset, new Vector3(-0.1f, Mathf.Clamp(Offset.y + ZoomInput, 0.1f, CameraZoomMax * 0.25f), Mathf.Clamp((Offset.z) + ZoomInput, CameraZoomMin, CameraZoomMax)), Time.deltaTime*125f);
        }
        ZoomInput = 0;

    }
    public void FixedUpdate()
    {
      
        if (!isLocalPlayer || !isClient) { return; }//is Not Server Check
        _Animator.SetFloat("Speed", RB.velocity.magnitude);
        if (Input.GetAxis("Horizontal") != 0 || 0 != Input.GetAxis("Vertical") || FJ.Horizontal != 0 || FJ.Vertical != 0)
        {
            if (!RB) { return; }

                float horizontalInput = Input.GetAxis("Horizontal");// X
                float verticalInput = Input.GetAxis("Vertical");// Z
                if(FJ.Horizontal!=0|| FJ.Vertical != 0) { horizontalInput = FJ.Horizontal; verticalInput = FJ.Vertical; }
                MoveOn(horizontalInput, verticalInput);
               // Debug.Log("isLocalMove: OnStart");
        }
    }
    public void LateUpdate()
    {
        if (!isLocalPlayer || !isClient) { return; }//is Not Server Check
        float horizontalInput = Input.GetAxis("HorizontalRotation"); // Получаем ввод для вращения
        float verticalInput = Input.GetAxis("VerticalRotation");

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (Input.touchCount == 2 && Input.GetAxis("Horizontal")==0 && Input.GetAxis("Vertical")==0)
            {
                horizontalInput = 0f;
                verticalInput = 0f;
            }
            if (Input.GetMouseButton(0)) { cameraspeed = CameraSpeed; } else { cameraspeed = 0; }
        }
        else
        {
            if (Input.GetMouseButton(1)) { cameraspeed = CameraSpeed; } else { cameraspeed = 0; }
        }

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
