using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnRolling : MonoBehaviour
{
    Rigidbody2D rb;
    AudioSource source;
    [SerializeField] ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        //ps = GetComponent<ParticleSystem>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
            if (rb != null && source != null && Mathf.Abs(rb.velocity.x) > 9 && !source.isPlaying)
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
