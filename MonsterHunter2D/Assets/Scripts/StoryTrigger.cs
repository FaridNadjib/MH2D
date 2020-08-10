using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    [SerializeField] StoryManager manager;
    [SerializeField] int indexToTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerController>() != null)
        {
            manager.TriggerDialog(indexToTrigger);
            gameObject.SetActive(false);
        }
    }
}
