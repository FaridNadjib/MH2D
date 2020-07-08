using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    private Stack reachedCheckpoints = new Stack();

    private bool rockEventStarted = false;

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Vector2 pos = LoadLastCheckpoint();

            if (pos == null)
                return;

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().transform.position = pos;
        }
    }

    public void CheckpointReached(Vector2 checkPointPos)
    {
        reachedCheckpoints.Push(checkPointPos);
    }

    public Vector2 LoadLastCheckpoint()
    {
        Vector2 pos = new Vector2();
        try
        {
            pos = (Vector2)reachedCheckpoints.Peek();
        }
        catch
        {
            print("Keine Checkpoints vorhanden!");
            pos = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().transform.position;
        }

        return pos;
    }
}
