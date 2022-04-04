using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private WeaponAssaultRifle weapon;      // 현재 정보가 출력되는 무기
    /*●*/[SerializeField]
    /*●*/private Status status;      // 플레이어의 상태 (이동속도, 체력)

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName;     // 무기 이름
    [SerializeField]
    private Image imageWeaponIcon;      // 무기 아이콘
    [SerializeField]
    private Sprite[] spriteWeaponIcons;    // 무기 아이콘에 사용되는 sprite 배열

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo;   // 현재/최대 탄 수 출력 text

    [Header("Magazine")]
    [SerializeField]
    private GameObject magazineUIPrefab;    // 탄창 UI 프리펩
    [SerializeField]
    private Transform magazineParent;       // 탄창 UI가 배치되는 Panel
    
    private List<GameObject> magazineList;      // 탄창 UI 리스트

    /*●*/[Header("HP & BloodScreen UI")]
    /*●*/[SerializeField]
    /*●*/private TextMeshProUGUI textHP;     // 플레이어의 체력을 출력하는 text
    /*●*/[SerializeField]
    /*●*/private Image imageBloodScreen;     // 플레이어가 공격받았을 때 화면에 표시되는 Image
    /*●*/[SerializeField]
    /*●*/private AnimationCurve curveBloodScreen;

    private void Awake()
    {
        SetupWeapon();
        SetupMagazine();

        // 메소드가 등록되어 있는 이벤트 클래스 (weapon.xx)의
        // Invoke() 메소드가 호출될 때 등록된 메소드(매개변수)가 실행된다.
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
        /*●*/status.onHPEvent.AddListener(UpdateHPHUD);
    }
    private void SetupWeapon()
    {
        textWeaponName.text=weapon.WeaponName.ToString();
        imageWeaponIcon.sprite=spriteWeaponIcons[(int)weapon.WeaponName];
    }
    private void UpdateAmmoHUD(int currentAmmo,int maxAmmo)
    {
         textAmmo.text=$"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }
    
   private void SetupMagazine()
   {
       // weapon애 등록되어 있는 최대 탄창 개수만큼 Image Icon을 생성
       // magazineParent 오브젝트의 자식으로 등록 후 모두 비활성화/리스트에 저장
       magazineList=new List<GameObject>(); 
       for(int i=0;i<weapon.MaxMagazine;i++)
       {
           GameObject clone = Instantiate(magazineUIPrefab);
           clone.transform.SetParent(magazineParent);
           clone.SetActive(false);
   
           magazineList.Add(clone);
       }
       // weapon에 등록되어 있는 현재 탄창 개수만큼 오브젝트 활성화
       for(int i=0;i<weapon.CurrentMagazine;i++)
       {
           magazineList[i].SetActive(true);
       }
   }

    private void UpdateMagazineHUD(int currentmagazine)
    {
        // 전부 비활성화하고, currentMagazine 개수만큼 활성화
        for(int i=0; magazineList.Count > i; i++)
        {
            magazineList[i].SetActive(false);
        }
        for(int i = 0; i < currentmagazine; i++)
        {
            magazineList[i].SetActive(true);
        }
    }

    /*●*/private void UpdateHPHUD(int previous,int current)
    /*●*/{
    /*●*/    textHP.text = "HP " + current;
    /*●*/
    /*●*/    if(previous - current > 0)
    /*●*/    {
    /*●*/        StopCoroutine("OnBloodScreen");
    /*●*/        StartCoroutine("OnBloodScreen");
    /*●*/    }
    /*●*/}
    /*●*/private IEnumerator OnBloodScreen()
    /*●*/{
    /*●*/    float percent = 0;
    /*●*/
    /*●*/    while (percent < 1)
    /*●*/    {
    /*●*/        percent+=Time.deltaTime;
    /*●*/
    /*●*/        Color color = imageBloodScreen.color;
    /*●*/        color.a=Mathf.Lerp(1,0,curveBloodScreen.Evaluate(percent));
    /*●*/        imageBloodScreen.color=color;
    /*●*/
    /*●*/        yield return null;
    /*●*/    }
    /*●*/}
}