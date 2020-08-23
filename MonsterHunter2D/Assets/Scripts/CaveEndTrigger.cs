using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEndTrigger : MonoBehaviour
{
    [SerializeField] private GameObject boulder;
    [SerializeField] private GameObject door;

    private bool triggeredOnce = false;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null && !triggeredOnce)
        {
            door.SetActive(false);
            boulder.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }   
    }
}
