using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private ParticleSystem waterSplash;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<EnemyFish>() != null || other.GetComponent<PlayerController>() != null )
        {
            waterSplash.transform.position = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y - 0.5f);
            waterSplash.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.GetComponent<EnemyFish>() != null || other.GetComponent<PlayerController>() != null)
        {
            waterSplash.transform.position = new Vector3 (other.gameObject.transform.position.x, other.gameObject.transform.position.y - 1);
            waterSplash.Play();
        }
    }
}
