using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{

    [SerializeField] float startFallTime;
    [SerializeField] float reappearTime;
    float timer;
    float timer2;
    bool startFall = false;
    Vector3 startPosition;
    Rigidbody2D rb;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        startPosition = transform.position;
        if (rb != null)
            rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startFall)
        {
            if (timer < startFallTime)
                timer += Time.deltaTime;
            else
            {
                rb.isKinematic = false;
                if (source != null)
                    source.Play();
            }

            if(timer2 < reappearTime)
            {
                timer2 += Time.deltaTime;
            }
            else
            {
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
                transform.position = startPosition;
                startFall = false;
                Debug.Log("resettet");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            startFall = true;
            timer = 0;
            timer2 = 0;
        }
    }
}
