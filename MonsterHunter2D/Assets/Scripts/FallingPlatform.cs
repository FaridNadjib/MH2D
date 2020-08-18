using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles our falling platforms. Farid.
/// </summary>
public class FallingPlatform : MonoBehaviour
{
    #region Fields
    [Header("Falling platfroms:")]
    [Tooltip("After how many seconds after colliding with player should the platform fall down?")]
    [SerializeField] float startFallTime;
    [Tooltip("After how many seconds after falling down should it reappear?")]
    [SerializeField] float reappearTime;

    // Countervariables and default components.
    float timer;
    float timer2;
    bool startFall = false;
    Vector3 startPosition;
    Rigidbody2D rb;
    AudioSource source;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Set the default values.
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        startPosition = transform.position;
        if (rb != null)
            rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Handle the timer and the the rigidbodys kinematic value of the platform either to true or false. Reset the position.
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
                timer2 += Time.deltaTime;
            else
            {
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
                transform.position = startPosition;
                startFall = false;
            }
        }
    }

    /// <summary>
    /// Checks if the player triggered the platform. It will start falling then.
    /// </summary>
    /// <param name="collision">Checks for the player.</param>
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
