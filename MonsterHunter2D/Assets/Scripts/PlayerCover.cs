using UnityEngine;

/// <summary>
/// A cover for the player that lets him become invisible for some seconds after crouching behind it.
/// </summary>
[RequireComponent(typeof(BoxCollider2D), typeof(AudioSource))]
public class PlayerCover : MonoBehaviour
{
    private AudioSource audioSource;
    private bool startedSound = false;
    private bool soundplaying = false;
    private float soundTimer = 0.5f;
    [SerializeField] private float soundDelayTime = 1f;
    private PlayerController player;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInsideBush(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerInsideBush(other);
    }

    private void PlayerInsideBush(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (player == null)
                player = other.GetComponent<PlayerController>();

            player.InsideBush = true;

            if (!audioSource.isPlaying && !startedSound)
            {
                audioSource.Play();
                startedSound = true;
            }
            else if (!audioSource.isPlaying && startedSound && player.MoveInput != 0)
                audioSource.UnPause();
            else if (audioSource.isPlaying && startedSound && Mathf.Approximately(player.MoveInput, 0))
                soundplaying = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (player == null)
                player = other.GetComponent<PlayerController>();

            player.InsideBush = false;
            soundplaying = true;
        }
    }

    private void Update()
    {
        if (!soundplaying)
            return;

        soundTimer += Time.deltaTime;

        if (soundTimer < soundDelayTime)
            return;

        soundTimer = 0f;
        soundplaying = false;
        audioSource.Pause();
    }
}
