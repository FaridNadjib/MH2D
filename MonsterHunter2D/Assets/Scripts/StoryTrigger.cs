using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers a index from the dialog array from the storymanager to show the corresponding lines.
/// </summary>
public class StoryTrigger : MonoBehaviour
{
    [SerializeField] StoryManager manager;
    [SerializeField] int indexToTrigger;

    /// <summary>
    /// Triggers the dialog from story manager.
    /// </summary>
    /// <param name="collision">The Player.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerController>() != null)
        {
            manager.TriggerDialog(indexToTrigger);
            gameObject.SetActive(false);
        }
    }
}
