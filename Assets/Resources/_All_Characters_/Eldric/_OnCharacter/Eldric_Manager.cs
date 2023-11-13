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
    [Header("0 - ������� . 1 - ������ . 2 - ������ . 3 - ���������� . 4 - ���� . 5 - �������� ")]
    public int BasicCharacterType = 0;
    #endregion
    private void Start()
    {
        _humanoid_ humanoid = transform.parent.parent.GetComponent<_humanoid_>();
        humanoid.variables.MaxHealth = BasicHealth;
        humanoid.variables.Health = BasicHealth;
        humanoid.variables.Defence = BasicDefence;
        humanoid.variables.Speed = BasicSpeed;
        humanoid.variables.CritRarity = BasicCrit;
        humanoid.variables.CritPower = BasicCritDamage;
        humanoid.variables.PhysicPower = BasicPhysicPower;
        humanoid.variables.MagicPower = BasicMagicPower;
        humanoid.variables.Defence = BasicDefence;
        humanoid.variables.CharacterType = BasicCharacterType;
    }

    [Command(requiresAuthority = false)]
    public void OnAttack()
    {

    }
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
