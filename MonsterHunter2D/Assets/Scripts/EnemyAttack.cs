using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private Enemy enemy;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (enemy.CanHit)
            {
                enemy.CanHit = false;
                other.GetComponent<CharacterResources>().ReduceHealth(damage);
            }
        }
    }
}