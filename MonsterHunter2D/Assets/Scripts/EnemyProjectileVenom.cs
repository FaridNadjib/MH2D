using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileVenom : EnemyProjectile
{
    protected new const string pool = "snakeVenomPool";

    protected override void SetProjectileLayer(){}
    protected override void SetGroundLayer(){}

    protected override void ConnectToCollisionObject(Collision2D collision)
    {
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

            GetComponent<AudioSource>().Play();
            GetComponent<Collider2D>().enabled = false;
            rb.isKinematic = true;
            rb.freezeRotation = true;
            onlyOnce = true;
        }
    }
}