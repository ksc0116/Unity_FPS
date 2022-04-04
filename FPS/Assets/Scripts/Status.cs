using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*¡Ü*/[System.Serializable]
/*¡Ü*/public class HPEvent : UnityEvent<int,int> {  }
public class Status : MonoBehaviour
{
    /*¡Ü*/[HideInInspector]
    /*¡Ü*/public HPEvent onHPEvent=new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;

    /*¡Ü*/[Header("HP")]
    /*¡Ü*/[SerializeField]
    /*¡Ü*/private int maxHP = 100;
    /*¡Ü*/private int currentHP;

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;

    /*¡Ü*/public int CurrentHP => currentHP;
    /*¡Ü*/public int MaxHP => maxHP;
    /*¡Ü*/
    /*¡Ü*/private void Awake()
    /*¡Ü*/{
    /*¡Ü*/    currentHP = maxHP;
    /*¡Ü*/}

    /*¡Ü*/public bool DecreaseHP(int damage)
    /*¡Ü*/{
    /*¡Ü*/    int previousHP = currentHP;
    /*¡Ü*/
    /*¡Ü*/    currentHP=currentHP- damage> 0 ? currentHP - damage : 0;
    /*¡Ü*/
    /*¡Ü*/    onHPEvent.Invoke(previousHP,currentHP);
    /*¡Ü*/
    /*¡Ü*/    if (currentHP == 0)
    /*¡Ü*/    {
    /*¡Ü*/        return true;
    /*¡Ü*/    }
    /*¡Ü*/
    /*¡Ü*/    return false;
    /*¡Ü*/}
}