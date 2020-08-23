using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activates an object on trigger enter.
/// </summary>
public class TriggerActivte : MonoBehaviour
{
    [SerializeField] GameObject ObjectToActivate;

    /// <summary>
    /// Activates the object if the palyer entered a trigger.
    /// </summary>
    /// <param name="collision">The player.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(ObjectToActivate.activeSelf == false)
                ObjectToActivate.SetActive(true);
        }
    }
}
