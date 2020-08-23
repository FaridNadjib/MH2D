using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the player a child of a moving platform.
/// </summary>
public class MovingPlatformsChildPlayer : MonoBehaviour
{
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Sets the player as child of the platform.
    /// </summary>
    /// <param name="collision">The player.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(ps != null && !ps.isEmitting)
                ps.Play();
            collision.transform.parent = transform.parent;
        }
    }

    /// <summary>
    /// Delets the player as child.
    /// </summary>
    /// <param name="collision">The player.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
            collision.transform.rotation = Quaternion.identity;
        }
    }
}
