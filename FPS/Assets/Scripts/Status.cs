using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HPEvent : UnityEvent<int,int> {  }
public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent=new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;

    [Header("HP")]
    [SerializeField]
    private int maxHP = 100;
    private int currentHP;

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;
    
    private void Awake()
    {
        currentHP = maxHP;
    }

    public bool DecreaseHP(int damage)
    {
        int previousHP = currentHP;
    
        currentHP=currentHP- damage> 0 ? currentHP - damage : 0;
    
        onHPEvent.Invoke(previousHP,currentHP);
    
        if (currentHP == 0)
        {
            return true;
        }
    
        return false;
    }

    /*¡Ü*/public void IncreaseHP(int hp)
    /*¡Ü*/{
    /*¡Ü*/    int previousHP=currentHP;
    /*¡Ü*/    currentHP=currentHP+hp > maxHP ? maxHP : currentHP + hp;
    /*¡Ü*/
    /*¡Ü*/    onHPEvent.Invoke(previousHP,currentHP);
    /*¡Ü*/}
}