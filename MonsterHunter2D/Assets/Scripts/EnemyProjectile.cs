using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    [SerializeField] protected string pool; 

    protected override void SetProjectileLayer(){}
 
    protected override void SetAsChildOfCharacter(Collision2D collision)
    {
        if (!onlyOnce && collision.gameObject.GetComponent<PlayerController>() != null)
        {
            transform.SetParent(collision.gameObject.transform);
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            onlyOnce = true;
            collision.gameObject.GetComponent<CharacterResources>().ReduceHealth(damage);

            if (gameObject.GetComponent<Collider2D>() != null)
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    protected override void ConnectToCollisionObject(Collision2D collision)
    {
        // For spear and arrow types, make them connect via joint with the collision object.
        if (!onlyOnce)
        {
            if (collision.gameObject.GetComponent<ProjectilesWillBounceFromMe>() != null)
            {
                // Gameobjects with that empty script attached to them wont allow projectiles to create joints with them.
                //rb.isKinematic = true;
                //transform.SetParent(collision.gameObject.transform);
                //gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }

            GetComponent<Collider2D>().enabled = false;
            rb.isKinematic = true;
            rb.freezeRotation = true;
            onlyOnce = true;
        }
    }

    protected override void SetGroundLayer(){}

    protected override void AddToPool()
    {
        if (pool != null)
            ObjectPoolsController.instance.AddToPool(gameObject, pool);
    }
}