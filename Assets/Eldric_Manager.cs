using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Eldric_Manager : NetworkBehaviour
{
    #region Vars
    [Header("Variables")]
    public float BasicHealth = 250f;
    public float BasicDefence = 10f;
    public float BasicPhysicPower = 1f;
    public float BasicMagicPower = 25f;
    public float BasicSpeed = 120f;
    public float BasicCrit = 0f;
    public float BasicCritDamage = 100f;
    [Header("0 - Человек . 1 - Нежить . 2 - Чудища . 3 - Приведение . 4 - Меха . 5 - Растения ")]
    public float BasicCharacterType = 0;
    #endregion


    [Command(requiresAuthority =false)]
    public void OnSpec1()
    {

    }
    [Command(requiresAuthority = false)]
    public void OnSpec2()
    {

    }
    [Command(requiresAuthority = false)]
    public void OnSpec3()
    {

    }
}
