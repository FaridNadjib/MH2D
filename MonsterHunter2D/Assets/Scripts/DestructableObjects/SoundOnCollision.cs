using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    AudioSource source;
    //bool canPlay = true;
    //[SerializeField] float soundPlayIntervall;
    //float timer;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
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
        if (source != null && !source.isPlaying)
        {
            source.Play();
            //canPlay = false;
        }
    }

}
