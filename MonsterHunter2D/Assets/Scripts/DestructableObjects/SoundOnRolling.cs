using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lets the rolling boulder play a sound as long as it has a certain velocity. Farid.
/// </summary>
public class SoundOnRolling : MonoBehaviour
{
    #region Fields
    Rigidbody2D rb;
    AudioSource source;
    TrapDamageArea trap;
    [SerializeField] ParticleSystem ps;
    [SerializeField] float activationVelocity;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        trap = gameObject.GetComponent<TrapDamageArea>();   
    }

    private void Update()
    {
        // Sets the trap area to true, ie deal damage once a certain velocity is reached.
        if (Mathf.Abs(rb.velocity.x) > activationVelocity)
        {
            if (trap != null)
                trap.TrapActive = true;
        }
        else
            trap.TrapActive = false;
    }

    /// <summary>
    /// Plays a sound and emmits a particle as long as the rigidbody has a certain velocity.
    /// </summary>
    /// <param name="collision">The ground the object is rolling on.</param>
    private void OnCollisionStay2D(Collision2D collision)
    {
            if (rb != null && source != null && Mathf.Abs(rb.velocity.x) > activationVelocity && !source.isPlaying)
            {
                source.Play();
                if (ps != null && !ps.isEmitting)
                {
                    ps.transform.position = collision.contacts[0].point;
                    ps.Play();
                }
            }        
    }
}
