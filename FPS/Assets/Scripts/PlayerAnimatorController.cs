using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator anim;
    private void Awake()
    {
        // "Player" 오브젝트 기준으로 자식 오브젝트인 
        // "arms_assault_rifle_01" 오브젝트에 Animator 컴포넌트 있음
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

    // Assault Rifle 마우스 오른쪽 클릭 액션 (default / aim mode)
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