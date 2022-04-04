using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBarrel : InteractionObject
{
    public override void TakeDamage(int damage)
    {
       
    }
    // TIP) 태그가 "InteractionObject"인 모든 상호작용 오브젝트는 
    // InteractionObject.TakeDamage();를 호출하기 때문에
    // 기본 드럼통도 InteractionObject 클래스를 상속받는 클래스를 제작한다.
    // 기본 드럼통은 설정상 체력이 무한해서 부서지지 않기 때문에 체력이 닳지 않도록 TakeDamage() 내부가 비어있다.
}
