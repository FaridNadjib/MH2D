using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderOffTrigger : MonoBehaviour
{
    private bool triggeredOnce = false;
    [SerializeField] private BoulderEvent boulderEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null && !triggeredOnce)
        {
            boulderEvent.DeActivate();
        }

    }
}
