using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision2 : MonoBehaviour
{
    AudioSource source;
    bool canPlay = true;

    ParticleSystem ps;
    //bool canPlay = true;
    //[SerializeField] float soundPlayIntervall;
    //float timer;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        ps = GetComponent<ParticleSystem>();
        //timer = soundPlayIntervall;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!canPlay)
        //    timer -= Time.deltaTime;
        //if(timer < 0)
        //{
        //    timer = soundPlayIntervall;
        //    canPlay = true;
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (source != null && !source.isPlaying && canPlay)
        {
            source.Play();
            
            canPlay = false;
            //canPlay = false;
        }
        if (ps != null)
            ps.Play();
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        canPlay = true;
    }
}
