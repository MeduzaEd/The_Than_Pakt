using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class _Character_Controller_ : NetworkBehaviour
{
    [SerializeField]
    private Camera C;
    [SerializeField]
    private CharacterController CC;
    private Vector3 Offset = new Vector3(-0.1f, 1.2f, -1.1f);
    [SyncVar]
    public bool InputLag = false;
    [SyncVar]
    public float PlayerSpeed=5f;
    private void Start()
    {
        CC = this.GetComponentInChildren<CharacterController>();
        C = GameObject.FindObjectOfType<Camera>();
    }
    public void FixedUpdate()
    {

        if (!isLocalPlayer || !isClient) { return; }//is Not Server Check
        if (Input.GetAxis("Horizontal") != 0 || 0 != Input.GetAxis("Vertical"))
        {
            if (!CC) { }
            else
            {
                float horizontalInput = Input.GetAxis("Horizontal");// X
                float verticalInput = Input.GetAxis("Vertical");// Z

                MoveOn(horizontalInput, verticalInput);
                Debug.Log("isLocalMove: OnStart");
            }
        }
    }
    public void LateUpdate()
    {
       
        if (!isLocalPlayer ||!isClient){return;}//is Not Server Check
        C.transform.position = CC.transform.position + Offset;
        
    }
    #region Move
    [Command]
    public void CmdMoveOn(float x, float z)
    {
        // Вызывайте метод движения на сервере
        if (!isServer) return;
        ServerMoveOn(x, z);
        //else { Debug.Log("isNotServer"); }
    }
    [Server]
    public void ServerMoveOn(float x, float z)
    {
        Vector3 Move = new Vector3(x, 0, z).normalized;
        Vector3 MoveNormal =Move* PlayerSpeed * Time.fixedDeltaTime;
        CC.Move(MoveNormal);
        Debuge(MoveNormal.ToString());
    }
    [ClientRpc]
    public void Debuge(string text)
    {
        Debug.Log($"isGlobalMove:{text},");
    }
    public void MoveOn(float x, float z)
    {
       // CC.Move(new Vector3(x, 0, z).normalized * PlayerSpeed * Time.smoothDeltaTime);
       
        CmdMoveOn(x, z);//Basic Move
    }
    #endregion

}
