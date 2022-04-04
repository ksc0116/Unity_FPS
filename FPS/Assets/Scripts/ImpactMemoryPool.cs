using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ImpactType { Normal=0, Obstacle, Enemy, /*��*/InteractionObject,}

public class ImpactMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] impactPrefab;      // �ǰ� ����Ʈ
    private MemoryPool[] memoryPool;        // �ǰ� ����Ʈ �޸�Ǯ

    private void Awake()
    {
        // �ǰ� ����Ʈ�� ���� ���� �̸� �������� �޸�Ǯ ����
        memoryPool = new MemoryPool[impactPrefab.Length];
        for(int i = 0; i < impactPrefab.Length; i++)
        {
            memoryPool[i]=new MemoryPool(impactPrefab[i]);
        }
    }

    public void SpawnImpact(RaycastHit hit)
    {
        // �ε��� ������Ʈ�� Tag ������ ���� �ٸ��� ó��
        if(hit.transform.tag == "ImpactNormal")
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.tag == "ImpactObstacle")
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
        }
       else if (hit.transform.tag == "ImpactEnemy")
       {
           OnSpawnImpact(ImpactType.Enemy,hit.point,Quaternion.LookRotation(hit.normal));
       }
       /*��*/else if (hit.transform.tag == "InteractionObject")
       /*��*/{
       /*��*/    Color color=hit.transform.GetComponentInChildren<MeshRenderer>().material.color;
       /*��*/    OnSpawnImpact(ImpactType.InteractionObject, hit.point, Quaternion.LookRotation(hit.normal),color);
       /*��*/}
    }

    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation,/*��*/Color color = new Color())
    {
        GameObject item=memoryPool[(int)type].ActivatePoolItem();
        item.transform.position=position;
        item.transform.rotation=rotation;
        item.GetComponent<Impact>().Setup(memoryPool[(int)type]);

        /*��*/if (type == ImpactType.InteractionObject)
        /*��*/{
        /*��*/    ParticleSystem.MainModule main= item.GetComponent<ParticleSystem>().main;
        /*��*/    main.startColor=color;
        /*��*/}
    }
}
