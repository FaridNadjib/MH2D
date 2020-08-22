using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerOn : MonoBehaviour
{
    private CameraTrigger trigger;

    private void Awake() 
    {
        trigger = GetComponentInParent<CameraTrigger>();   
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null && !trigger.ActivatedOnce)
        {
            trigger.On();
        }
    }
}
