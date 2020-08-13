using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterSounds : MonoBehaviour
{
    private AudioSource audioSource;

    public enum Sound { Trigger, Idle, Alerted, Moving, Jump, Land, MeleeAttacking, RangedAttacking, Shoot, Hit, Dead, Sliding, Walking, Running }

    [System.Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip[] audioClips;
    }

    [SerializeField] private SoundAudioClip[] soundAudioClips;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(Sound sound, int index, bool random, bool loop)
    {
        audioSource.clip = GetAudioClip(sound, index, random);
        
        if (loop)
            audioSource.loop = true;
        else
            audioSource.loop = false;

        audioSource.Play();
    }

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
        Debug.LogError("Sound " + sound + " not found");
        return null;
    }

    public void StopAll()
    {
        if (audioSource != null)
            audioSource.Stop();
    }

    public void SetNull()
    {
        audioSource.clip = null;
    }

    public void Stop(Sound sound)
    {
        if (audioSource != null && IsPlaying(sound))
            audioSource.Stop();
    }

    public void Pause()
    {
        if (audioSource != null)
            audioSource.Pause();
    }

    public void UnPause()
    {
        if (audioSource != null)
            audioSource.UnPause();
    }

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
