using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float damage;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            print("hit");
            //other.GetComponent<CharacterResources>().ReduceHealth(damage);
        }
    }
}
