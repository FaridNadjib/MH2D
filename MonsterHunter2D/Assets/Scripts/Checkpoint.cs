using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;


/// <summary>
/// This class handles our checkpoint totem. once activated, the player will respawn at the totems position.
/// Furthermore the player can get full ammo there. Joachim.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Checkpoint : MonoBehaviour
{
    #region Fields
    [Header("Checkpoint Light activation related:")]
    [SerializeField] private Light2D spriteLight;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float activatedIntensity;
    [SerializeField] private float transitionTime;
    [Header ("Checkpoint reaktivation related:")]
    [Tooltip("This icon needs a image with fill type, it displays when checkpoint can be r´ractivated again.")]
    [SerializeField] Image reactivationIcon;
    [Tooltip("After how many seconds can the player gather ammo again?")]
    [SerializeField] float reactivationTime;
    bool canGather = true;
    float timer = 0;

    private bool triggeredOnce = false;
    private bool triggered = false;
    private bool activated = false;
    private float t = 0f;
    #endregion

    private void Update()
    {
        Transition();
        TransitionBack();

        // Count if the player can gathaer ammo again.
        if (!canGather)
        {
            if(timer < reactivationTime)
            {
                timer += Time.deltaTime;
                reactivationIcon.fillAmount = timer / reactivationTime;
            }
            else
            {
                canGather = true;
                reactivationIcon.fillAmount = 0f;
            }
        }
    }

    /// <summary>
    /// Here we check if the player entered the trigger and set his new spawn position and refill his ammo.
    /// </summary>
    /// <param name="other">Checks for the player.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canGather && other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().RefeillAllValues();
            canGather = false;
        }

        if (triggeredOnce == true)
            return;

        // Do this only once.
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().SetSpawnPosition(transform.position.x, transform.position.y);
            triggeredOnce = true;
            triggered = true;
        }
    }

    /// <summary>
    /// Lets the checkpointsprite glow up.
    /// </summary>
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

    /// <summary>
    /// Makes the sprite return to normal.
    /// </summary>
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
