using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private WeaponAssaultRifle weapon;      // ���� ������ ��µǴ� ����

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName;     // ���� �̸�
    [SerializeField]
    private Image imageWeaponIcon;      // ���� ������
    [SerializeField]
    private Sprite[] spriteWeaponIcons;    // ���� �����ܿ� ���Ǵ� sprite �迭

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo;   // ����/�ִ� ź �� ��� text

    /*��*/[Header("Magazine")]
    /*��*/[SerializeField]
    /*��*/private GameObject magazineUIPrefab;    // źâ UI ������
    /*��*/[SerializeField]
    /*��*/private Transform magazineParent;       // źâ UI�� ��ġ�Ǵ� Panel
    /*��*/
    /*��*/private List<GameObject> magazineList;      // źâ UI ����Ʈ

    private void Awake()
    {
        SetupWeapon();
        /*��*/SetupMagazine();

        // �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ���� (weapon.xx)��
        // Invoke() �޼ҵ尡 ȣ��� �� ��ϵ� �޼ҵ�(�Ű�����)�� ����ȴ�.
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        /*��*/weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
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
    
    /*��*/private void SetupMagazine()
    /*��*/{
    /*��*/    // weapon�� ��ϵǾ� �ִ� �ִ� źâ ������ŭ Image Icon�� ����
    /*��*/    // magazineParent ������Ʈ�� �ڽ����� ��� �� ��� ��Ȱ��ȭ/����Ʈ�� ����
    /*��*/    magazineList=new List<GameObject>(); 
    /*��*/    for(int i=0;i<weapon.MaxMagazine;i++)
    /*��*/    {
    /*��*/        GameObject clone = Instantiate(magazineUIPrefab);
    /*��*/        clone.transform.SetParent(magazineParent);
    /*��*/        clone.SetActive(false);
    /*��*/
    /*��*/        magazineList.Add(clone);
    /*��*/    }
    /*��*/    // weapon�� ��ϵǾ� �ִ� ���� źâ ������ŭ ������Ʈ Ȱ��ȭ
    /*��*/    for(int i=0;i<weapon.CurrentMagazine;i++)
    /*��*/    {
    /*��*/        magazineList[i].SetActive(true);
    /*��*/    }
    /*��*/}

    /*��*/private void UpdateMagazineHUD(int currentmagazine)
    /*��*/{
    /*��*/    // ���� ��Ȱ��ȭ�ϰ�, currentMagazine ������ŭ Ȱ��ȭ
    /*��*/    for(int i=0; magazineList.Count > i; i++)
    /*��*/    {
    /*��*/        magazineList[i].SetActive(false);
    /*��*/    }
    /*��*/    for(int i = 0; i < currentmagazine; i++)
    /*��*/    {
    /*��*/        magazineList[i].SetActive(true);
    /*��*/    }
    /*��*/}
}