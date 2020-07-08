using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(BoxCollider2D))]
public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Light2D spriteLight;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float activatedIntensity;
    [SerializeField] private float transitionTime;
    [SerializeField] private GameObject levelManager;

    private bool triggeredOnce = false;

    private bool triggered = false;
    private bool activated = false;

    private float t = 0f;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (triggeredOnce == true)
            return;

        if (other.tag == "Player")
        {
            levelManager.GetComponent<LevelManager>().CheckpointReached(this.transform.position);
            triggeredOnce = true;
            triggered = true;
        }
    }

    private void Update()
    {
        Transition();
        TransitionBack();
    }

    private void Transition()
    {
        if (!triggered)
            return;

        t += transitionTime * Time.deltaTime / maxIntensity;

        spriteLight.intensity = Mathf.Lerp(0.5f, maxIntensity, t); 

        if (spriteLight.intensity == maxIntensity)
        {
            triggered = false;
            activated = true;
            t = 0f;
        }  
    }

    private void TransitionBack()
    {
        if (!activated)
            return;

        t += transitionTime * Time.deltaTime / maxIntensity;

        spriteLight.intensity = Mathf.Lerp(maxIntensity, activatedIntensity, t);

        if (spriteLight.intensity == activatedIntensity)
        {
            activated = false;
            t = 0f;
        }
    }
}
