using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterSounds : MonoBehaviour
{
    private AudioSource audioSource;

    public enum Sound { Trigger, Idle, Alerted, Moving, MeleeAttacking, RangedAttacking, Hit, Dead }

    [System.Serializable]
    public class SoundAudioClip
    {
        public Sound sound;
        public AudioClip[] audioClips;
    }

    [SerializeField] private SoundAudioClip[] soundAudioClips;

    public void PlaySound(Sound sound, int index, bool random, bool loop)
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
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
}
