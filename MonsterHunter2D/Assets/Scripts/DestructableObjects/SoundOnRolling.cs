using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnRolling : MonoBehaviour
{
    Rigidbody2D rb;
    AudioSource source;
    TrapDamageArea trap;
    [SerializeField] ParticleSystem ps;
    [SerializeField] float activationVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        //ps = GetComponent<ParticleSystem>();
        trap = gameObject.GetComponent<TrapDamageArea>();
        
    }

    private void Update()
    {
        if (Mathf.Abs(rb.velocity.x) > activationVelocity)
        {
            if (trap != null)
                trap.TrapActive = true;
        }
        else
        {
            trap.TrapActive = false;
        }
        //Debug.Log(Mathf.Abs(rb.velocity.x));
    }


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
