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

    public float CameraSpeed = 5f;

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

        float horizontalInput = Input.GetAxis("HorizontalRotation"); // Получаем ввод для вращения
        float verticalInput = Input.GetAxis("VerticalRotation");
        Debug.Log($"H: {horizontalInput} |V:{verticalInput}");
        Vector3 Rbp = RB.transform.position;


        C.transform.RotateAround(Rbp, Vector3.up, horizontalInput * CameraSpeed);

        // Вращение по вертикали с ограничением угла
        verticalRotation -= verticalInput * CameraSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, MinVerticalAngle, MaxVerticalAngle);

        C.transform.rotation = Quaternion.Euler(verticalRotation, C.transform.eulerAngles.y, 0);

        // Позиционирование камеры
        C.transform.position = Rbp - C.transform.forward * Offset.z + Vector3.up * Offset.y;
    
    }
    #region Move
    [Command]
    public void CmdMoveOn(float x, float z)
    {
        // Вызывайте метод движения на сервере
        if (!isServer) return;
        Vector3 Move = new Vector3(x, -.1f, z).normalized;
        Vector3 MoveNormal = Move * PlayerSpeed * Time.fixedDeltaTime;
        RpcDebuge(MoveNormal.ToString());
        RB.velocity = (MoveNormal);
        //ServerMoveOn(x, z);
        //else { Debug.Log("isNotServer"); }

    }
    [Server]
    public void ServerMoveOn(float x, float z)
    {
        Vector3 Move = new Vector3(x, -.1f, z).normalized;
        Vector3 MoveNormal =Move* PlayerSpeed * Time.fixedDeltaTime;
        RB.velocity = (MoveNormal);
        //Debuge(MoveNormal.ToString());
    }
    [ClientRpc]
    public void RpcDebuge(string text)
    {
        Debug.Log($"isGlobal:{text},");
    }
    public void MoveOn(float x, float z)
    {

        Vector3 Move = new Vector3(x, -.1f, z).normalized;
        Vector3 MoveNormal = Move * PlayerSpeed * Time.fixedDeltaTime;
        RpcDebuge(MoveNormal.ToString());
        RB.velocity=(MoveNormal);

        CmdMoveOn(x, z);//Basic Move

    }
    #endregion

}
