using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It plays a sound on Collision(outdated).
/// </summary>
public class SoundOnCollision2 : MonoBehaviour
{
    #region Fields
    AudioSource source;
    bool canPlay = true;

    ParticleSystem ps;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        ps = GetComponent<ParticleSystem>();
    }


    /// <summary>
    /// Plays a sound only once.
    /// </summary>
    /// <param name="collision">Any.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (source != null && !source.isPlaying && canPlay)
        {
            source.Play();          
            canPlay = false;
        }
        if (ps != null)
            ps.Play();
    }
    /// <summary>
    /// Resets the capability to replay the sound.
    /// </summary>
    /// <param name="collision">Any.</param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        canPlay = true;
    }
}
