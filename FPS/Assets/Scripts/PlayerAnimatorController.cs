using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator anim;
    private void Awake()
    {
        // "Player" ������Ʈ �������� �ڽ� ������Ʈ�� 
        // "arms_assault_rifle_01" ������Ʈ�� Animator ������Ʈ ����
        anim=GetComponentInChildren<Animator>();
    }

    public float MoveSpeed
    {
        set=>anim.SetFloat("movementSpeed",value);
        get => anim.GetFloat("movementSpeed");
    }
    public void OnReload()
    {
        anim.SetTrigger("onReload");
    }

    // Assault Rifle ���콺 ������ Ŭ�� �׼� (default / aim mode)
    public bool AimModeIs
    {
        set => anim.SetBool("isAimMode", value);
        get => anim.GetBool("isAimMode");
    }

   public void Play(string stateName,int layer, float normalizedTime)
   {
       anim.Play(stateName,layer,normalizedTime);
   }

    public bool CurrentAnimationIs(string name)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
}