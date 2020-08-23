using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Play a sound on collision.
/// </summary>
public class SoundOnCollision : MonoBehaviour
{
    #region Fields
    AudioSource source;
    CharacterSounds characterSounds;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        characterSounds = GetComponent<CharacterSounds>();
    }

    /// <summary>
    /// Plays a sound on collision.
    /// </summary>
    /// <param name="collision">Any.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (source != null && !source.isPlaying)
        {
            if(characterSounds != null)
                characterSounds.PlaySound(CharacterSounds.Sound.Collision, 0, true, false);

            source.Play();
        }
    }

}
