using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamageArea : MonoBehaviour
{
    [SerializeField] float recoilStrength;
    [SerializeField] float trapDamage;
    [SerializeField] ParticleSystem blood1;
    [SerializeField] ParticleSystem blood2;
    [SerializeField] AudioClip activationSound;
    AudioSource source;

    // TrapActive is default true, just for rolling boulder it gets set to false in the sound on rolling script, for every other trap its not important.
    public bool TrapActive { get; set; } = true;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null && TrapActive)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            //Vector3 direction = player.transform.position - (Vector3)collision.contacts[0].point;
            //Vector3 direction = (Vector3)collision.contacts[0].point - transform.position;
            Vector3 direction = player.transform.position - transform.position;
            player.ApplyRecoil(direction.normalized, recoilStrength);
            collision.gameObject.GetComponent<CharacterResources>().ReduceHealth(trapDamage);
            if (source != null)
                source.PlayOneShot(activationSound);
            if(blood1 != null)
            {
                blood1.transform.position = collision.contacts[0].point;
                blood1.Play();
            }
            if(blood2 != null)
            {
                blood2.transform.position = collision.contacts[0].point;
                blood2.Play();
            }
        }
    }

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (collision.gameObject.GetComponent<PlayerController>() != null)
    //    {
    //        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
    //        //Vector3 direction = player.transform.position - (Vector3)collision.contacts[0].point;
    //        Vector3 direction = player.transform.position - transform.position;
    //        player.ApplyRecoil(direction.normalized, recoilStrength);
    //        collision.gameObject.GetComponent<CharacterResources>().ReduceHealth(trapDamage);
    //    }
    //}
}
