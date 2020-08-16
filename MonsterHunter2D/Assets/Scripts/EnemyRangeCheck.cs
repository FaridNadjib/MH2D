using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeCheck : MonoBehaviour
{
    private Enemy enemy;

    private void Awake() 
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null && other.GetComponent<PlayerController>().Invisible == false)
        {
            enemy.PlayerInRange(other);
        }
    }
}
