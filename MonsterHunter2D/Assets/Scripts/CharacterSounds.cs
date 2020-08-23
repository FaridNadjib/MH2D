using UnityEngine;

/// <summary>
/// This class handles the various sounds a character can make. Joachim.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class CharacterSounds : MonoBehaviour
{
    #region Fields
    private AudioSource audioSource;

    /// <summary>
    /// The type of the sound.
    /// </summary>
    public enum Sound { Trigger, Idle, Alerted, Moving, Jump, Land, MeleeAttacking, RangedAttacking, Shoot, Hit, Dead, Sliding, Walking, Sprinting, Stealth, Collision }

    /// <summary>
    /// The clip and the type, to reference better in inspector.
    /// </summary>
    [System.Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip[] audioClips;
    }

    [SerializeField] private SoundAudioClip[] soundAudioClips;
    #endregion

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Play the sound.
    /// </summary>
    /// <param name="sound">Its type.</param>
    /// <param name="index">Its index.</param>
    /// <param name="random">If it should be picked by random.</param>
    /// <param name="loop">If it should loop.</param>
    public void PlaySound(Sound sound, int index, bool random, bool loop)
    {
        audioSource.clip = GetAudioClip(sound, index, random);
        
        if (loop)
            audioSource.loop = true;
        else
            audioSource.loop = false;

        audioSource.Play();
    }

    /// <summary>
    /// Return a clip to play.
    /// </summary>
    /// <param name="sound">Its type.</param>
    /// <param name="index">Its index.</param>
    /// <param name="random">If a random one should be picked.</param>
    /// <returns>The audioclip to play.</returns>
    private AudioClip GetAudioClip(Sound sound, int index, bool random)
    {
        foreach (SoundAudioClip soundAudioClip in soundAudioClips)
        {
            if (soundAudioClip.sound == sound)
            {
                if (!random)
                    return soundAudioClip.audioClips[index];
                else
                    return soundAudioClip.audioClips[UnityEngine.Random.Range(0, soundAudioClip.audioClips.Length)];
            }
        }
        return null;
    }

    /// <summary>
    /// Stops a specific sound by name.
    /// </summary>
    /// <param name="sound">The type.</param>
    public void StopSound(Sound sound)
    {
        if (audioSource != null && IsPlaying(sound))
            audioSource.Stop();
    }

    /// <summary>
    /// Stops all sounds.
    /// </summary>
    public void StopAllSounds()
    {
        audioSource.Stop();
    }

    /// <summary>
    /// Pauses the audiosource.
    /// </summary>
    public void Pause()
    {
        if (audioSource != null)
            audioSource.Pause();
    }

    /// <summary>
    /// Unpauses the audioSource.
    /// </summary>
    public void UnPause()
    {
        if (audioSource != null)
            audioSource.UnPause();
    }

    /// <summary>
    /// Checks if a specific clip is being played.
    /// </summary>
    /// <param name="sound">The type of the clip.</param>
    /// <returns>True if the clip is playing and false if its not.</returns>
    public bool IsPlaying(Sound sound)
    {
        if (audioSource.clip == null)
            return false;

        if (audioSource.clip == GetAudioClip(sound, 0, false) && audioSource.isPlaying)
            return true;
        else    
            return false;
    }
}
