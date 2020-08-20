using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStreamer : MonoBehaviour
{

    [SerializeField] GameObject ObjectToActivateRight;
    [SerializeField] GameObject ObjectToActivateLeft;
    [SerializeField] GameObject[] ObjectsToDeactivate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.transform.position.x > transform.position.x)
            {
                Debug.Log("Player left to the right.");
                DeactivateObjects();
                if (ObjectToActivateRight != null)
                    ObjectToActivateRight.SetActive(true);
            }
            else
            {
                Debug.Log("Player left to the left.");
                DeactivateObjects();
                if (ObjectToActivateLeft != null)
                    ObjectToActivateLeft.SetActive(true);
            }


        }
    }

    void DeactivateObjects()
    {
        for (int i = 0; i < ObjectsToDeactivate.Length; i++)
        {
            ObjectsToDeactivate[i].SetActive(false);
        }
    }
}
