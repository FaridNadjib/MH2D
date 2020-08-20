using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionDoor : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    [SerializeField] private GameManager gameManager;

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (Input.GetKeyDown(KeyCode.E))
            gameManager.LoadLevel(sceneIndex);
    }
}
