using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Humanoid : NetworkBehaviour
{
    // 0 - Человек . 1 - Нежить . 2 - Чудища . 3 - Приведение . 4 - Меха . 5 - Растения 

    #region Async Vars
    [Header("Async Vars"),SyncVar]
    public float Health = 10f;
    [SyncVar]
    public float MaxHealth = 20f;
    [SyncVar]
    public float Mana = 9f;
    [SyncVar]
    public float MaxMana = 10f;
    [SyncVar]
    public float Damage = 1f;
    [SyncVar]
    public float Defence = 1f;
    [SyncVar]
    public float Speed = 5f;
    [SyncVar]
    public bool OnLoaded = false;
    [SyncVar]
    public bool OnDied = false;
    [SyncVar]
    public bool OnImmune = false;
    [SyncVar]
    public bool OnImmortal = false;
    [Header("0 - Человек . 1 - Нежить . 2 - Чудища . 3 - Приведение . 4 - Меха . 5 - Растения "), SyncVar]
    public int CharacterType = 0;    // 0 - Человек . 1 - Нежить . 2 - Чудища . 3 - Приведение . 4 - Меха . 5 - Растения .
    public int Team = 0;//0 - Players . 1 - NPC . 2 -Enemy.
    #endregion
    #region Get Price
    [SyncVar]
    public int Exp = 0;
    [SyncVar]
    public int Gold = 0;

    #endregion
    [Command]
    public void _OnDamage(Transform own)
    {
        float _Damage = own.GetComponent<Humanoid>().Damage;

        if (_Damage >= 0)
        { Health = 1f > _Damage - Defence ? Health - 1f : Health - (_Damage - Defence); }
        else
        { Health += _Damage; }

        if(Health<=0&& OnImmortal ==false && OnImmune == false)
        {
            _OnDied(own);
        }
    }

    [Command]
    public void _OnDied(Transform own)
    {
        OnDied = true;
        
    }


    #region Local Variables 

    RectTransform UI;


    #endregion
    private void Start()
    {
         UI = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (Health < MaxHealth)
        {
            UI.localPosition = new Vector3(Health <= 0 ? 0f : ((Health / MaxHealth) * 50f)-50f, 0, 0);
        }
        else
        {
            UI.localPosition = new Vector3(-50f, 0, 0);
        }
    }
}
