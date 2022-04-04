using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class AmmoEvent : UnityEvent<int, int> { }

[System.Serializable]
public class MagazineEvent : UnityEvent<int> { }

public class WeaponAssaultRifle : MonoBehaviour
{
    [HideInInspector]
    public AmmoEvent onAmmoEvent=new AmmoEvent();

    [HideInInspector]
    public MagazineEvent onMagazineEvent=new MagazineEvent();

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect;      // �ѱ� ����Ʈ (On/Off)

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint;     // ź�� ���� ��ġ
    [SerializeField]
    private Transform bulletSpawnPoint;    // �Ѿ� ���� ��ġ

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon;           // ���� ���� ����
    [SerializeField]
    private AudioClip audioClipFire;        // ���� ����
    [SerializeField]
    private AudioClip audioClipReload;      // ������ ����

    [Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting;              // ���� ����

    [Header("Aim UI")]
    [SerializeField]
    private Image imageAim;     // default / aim ��忡 ���� Aim �̹��� Ȱ�� / ��Ȱ��

    private float lastAttackTime = 0;                       // ������ �߻�ð� üũ��
    private bool isReload = false;          // ������ ������ üũ
    private bool isAttack=false;        // ���� ���� üũ��
    private bool isModeChange = false;      // ��� ��ȯ ���� üũ��
    private float defaultModeFOV = 60f;      // �⺻��忡���� ī�޶� FOV
    private float aimModeFOV = 30f;          // AIM��忡���� ī�޶� FOV

    private AudioSource          audioSource;            // ���� ��� ������Ʈ
    private PlayerAnimatorController anim;             // �ִϸ��̼� ��� ����
    private CasingMemoryPool casingMemoryPool;      // ź�� ���� �� Ȱ��/��Ȱ�� ����
   private ImpactMemoryPool impactMemoryPool;      // ���� ȿ�� ���� �� Ȱ��/��Ȱ�� ����
   private Camera mainCamera;      // ���� �߻�

    // �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get ������Ƽ��
    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine=>weaponSetting.maxMagazine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInParent<PlayerAnimatorController>();
        casingMemoryPool=GetComponent<CasingMemoryPool>();
        impactMemoryPool=GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        // ó�� źâ ���� �ִ�� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;

        // ó�� ź ���� �ִ�� ����
        weaponSetting.currentAmmo=weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        // ���� ���� ���� ���
        PlaySound(audioClipTakeOutWeapon);
        // �ѱ� ����Ʈ ������Ʈ ��Ȱ��ȭ
        muzzleFlashEffect.SetActive(false);

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ źâ ������ �����Ѵ�.
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ �����Ѵ�.
        onAmmoEvent.Invoke(weaponSetting.currentAmmo,weaponSetting.maxAmmo);

        ResetVariables();
    }

    // �ܺο��� ���� ������ �� StartWeaponAction(0) �޼ҵ� ȣ��
    public void StartWeaponAction(int type = 0)
    {
        // ������ ���� ���� ���� �׼��� �� �� ����.
        if (isReload == true) return;

        // ��� ��ȯ���̸� ���� �׼��� �� �� ����.
        if (isModeChange == true) return;

        // ���콺 ���� Ŭ�� (���� ����)
        if (type == 0)
        {
            // ���� ����
            if (weaponSetting.isAutomaticAttack == true)
            {
                isAttack = true;
                // OnAttack()�� �� ������ ����
                StartCoroutine("OnAttackLoop");
            }
            // �ܹ� ����
            else
            {
                // ���� ���� ���� �Լ�
                OnAttack();
            }
        }
        // ���콺 ������ Ŭ�� (��� ��ȯ)
        else
        {
            // ���� ���� ���� ��� ��ȯ�� �� �� ����
            if (isAttack == true) return;
        
            StartCoroutine("OnModeChange");
        }
    }
    public void StartReload()
    {
        // ���� ������ ���̸� ������ �Ұ���
        if(isReload == true || weaponSetting.currentMagazine<=0) return;
    
        // ���� �׼� ���߿� 'R'Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
        StopWeaponAction();
    
        StartCoroutine("OnReload");
    }
    // �ܺο��� ���� ������ �� StopWeaponAction(0) �޼ҵ� ȣ��
    public void StopWeaponAction(int type = 0)
    {
        // ���콺 ���� ���� (���� ����)
        if(type == 0)
        {
            isAttack=false;
            StopCoroutine("OnAttackLoop");
        }
    }

    private IEnumerator OnAttackLoop()
    {
        while (true)
        {
            OnAttack();
    
            yield return null;
        }
    }

    public void OnAttack()
    {
        if(Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            // �ٰ����� �� ����x
            if (anim.MoveSpeed > 0.5f)
            {
                return;
            }
    
            // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ����ð� ���� ����
            lastAttackTime = Time.time;

            // ź ���� ������ ���� �Ұ���
            if (weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            // ���� �� currentAmmo 1 ����, ź �� UI ������Ʈ
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo,weaponSetting.maxAmmo);

            // ���� �ִϸ��̼� ���
            //anim.Play("Fire",-1,0);
            // ���� �ִϸ��̼� ��� (��忡 ���� AimFire or Fire �ִϸ��̼� ���)
            string animation = anim.AimModeIs == true ? "AimFire" : "Fire";
            anim.Play(animation, - 1, 0);
            // TIP) anim.Play("Fire"); : ���� �ִϸ��̼��� �ݺ��� �� �߰��� ���� ���ϰ� ��� �Ϸ� �� �ٽ� ���
            // TIP) anim.Play("Fire",-1,0); : ���� �ִϸ��̼��� �ݺ��� �� �ִϸ��̼��� ���� ó������ �ٽ� ���

            // �ѱ� ����Ʈ ��� (default ��� �� ���� ���)
            if(anim.AimModeIs==false) StartCoroutine("OnMuzzleFlashEffect");
            // ���� ���� ���
            PlaySound(audioClipFire);
            // ź�� ����
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            // ������ �߻��� ���ϴ� ��ġ ���� (+Impact Effect)
            TwoStepRaycast();
        }
    }
    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive (true);
    
        // ������ ���ݼӵ����� ������ ��� Ȱ��ȭ ���״ٰ� ��Ȱ��ȭ �Ѵ�
        yield return new WaitForSeconds(weaponSetting.attackRate*0.3f);
    
        muzzleFlashEffect.SetActive(false);
    }

    private IEnumerator OnReload()
    {
        isReload = true;
    
        // ������ �ִϸ��̼� ���� ���
        anim.OnReload();
        audioSource.PlayOneShot(audioClipReload,1.0f); // ���尡 ���ĵ� ����� �ǵ���
    
        while (true)
        {
            // ���尡 ������� �ƴϰ�, ���� �ִϸ��̼� Movement�̸�
            // ������ �ִϸ��̼�(, ����) ����� ����Ǿ��ٴ� ��
            if(audioSource.isPlaying==false && (anim.CurrentAnimationIs("Movement") || anim.CurrentAnimationIs("AimFirePose")))
            {
                isReload=false;

                // ���� źâ ���� 1 ���ҽ�Ű��, �ٲ� źâ ������ Text UI�� ������Ʈ
                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                // ���� ź ���� �ִ�� �����ϰ�, �ٲ� �� �� ������ Text UI�� ������Ʈ
                weaponSetting.currentAmmo=weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    
                yield break;
            }
            yield return null;
        }
    }
    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;
    
        // ȭ�� �߾� ��ǥ (Aim �������� Raycast����)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        // ���� ��Ÿ�(attackDistance) �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� ������ �ε��� ��ġ
        if(Physics.Raycast(ray,out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
            impactMemoryPool.SpawnImpact(hit);
    
        }
        // ���� ��Ÿ� �ȿ�  �ε����� ������Ʈ�� ������ targetPoint�� �ִ� ��Ÿ� ��ġ
        else
        {
            targetPoint=ray.origin+ray.direction*weaponSetting.attackDistance;
        }
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);
    
        // ù��° Raycast�������� ����� targetPoint�� ��ǥ�������� �����ϰ�,
        // �ѱ��� ������������ �Ͽ� Raycast ����
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);

            if (hit.transform.tag == "ImpactEnemy")
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            /*��*/else if (hit.transform.tag == "InteractionObject")
            /*��*/{
            /*��*/    hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            /*��*/}
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
    }
    private IEnumerator OnModeChange()
    {
        float current = 0;
        float percent = 0;
        float time = 0.35f;
    
        anim.AimModeIs = !anim.AimModeIs;
        imageAim.enabled = !imageAim.enabled;
    
        float start = mainCamera.fieldOfView;
        float end = anim.AimModeIs==true? aimModeFOV : defaultModeFOV;
    
        isModeChange = true;
    
        while (percent < 1)
        {
            current+=Time.deltaTime;
            percent=current/time;
    
            // mode�� ���� ī�޶��� �þ߰��� ����
            mainCamera.fieldOfView = Mathf.Lerp(start,end,percent);
    
            yield return null;
        }
        isModeChange=false;
    }

    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;   
        isModeChange=false;
    }
    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();             // ������ ������� ���带 �����ϰ�,
        audioSource.clip = clip;        // ���ο� ���� clip���� ��ü ��
        audioSource.Play();             // ���� ���
    }
}