using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class handles the damage and recoil made by traps.
/// </summary>
public class TrapDamageArea : MonoBehaviour
{
    #region Fields
    [Tooltip("How strong should the applied force be?")]
    [SerializeField] float recoilStrength;
    [SerializeField] float trapDamage;
    [SerializeField] AudioClip activationSound;
    AudioSource source;
    #endregion

    #region Properties
    // TrapActive is default true, just for rolling boulder it gets set to false in the sound on rolling script, for every other trap its not important.
    public bool TrapActive { get; set; } = true;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    /// <summary>
    /// If the player collides with a trap damage is dealt to him and he gets a push back.
    /// </summary>
    /// <param name="collision">Checks for the player.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null && TrapActive)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            Vector3 direction = player.transform.position - transform.position;
            Vector2 pos = collision.GetContact(0).point;
            player.ApplyRecoil(direction.normalized, recoilStrength, pos, true);
            collision.gameObject.GetComponent<CharacterResources>().ReduceHealth(trapDamage);
            if (source != null)
                source.PlayOneShot(activationSound);
        }
    }

}
