using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Item")
        {
            other.transform.GetComponent<ItemBase>().Use(transform.parent.gameObject);
        }
    }
}