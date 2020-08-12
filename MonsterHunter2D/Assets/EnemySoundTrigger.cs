using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundTrigger : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    private bool triggeredOnce = false;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null && !triggeredOnce)
        {
            triggeredOnce = true;
            enemy.PlayTriggerSound();
        }
    }
}
