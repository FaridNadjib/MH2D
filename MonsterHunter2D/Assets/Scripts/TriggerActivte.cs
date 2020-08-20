using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActivte : MonoBehaviour
{
    [SerializeField] GameObject ObjectToActivate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player did entered.");
            if(ObjectToActivate.activeSelf == false)
                ObjectToActivate.SetActive(true);
        }
    }
}
