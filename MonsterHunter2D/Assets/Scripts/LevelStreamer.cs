using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class deactivates objects and then activates some, depending on which side the player left a trigger. We wanted to improve the performance a little bit.
/// </summary>
public class LevelStreamer : MonoBehaviour
{
    #region Fields
    [SerializeField] GameObject ObjectToActivateRight;
    [SerializeField] GameObject ObjectToActivateLeft;
    [SerializeField] GameObject[] ObjectsToDeactivate;
    #endregion

    /// <summary>
    /// Which way did the player left the trigger? Activate the object in the direction he is going,
    /// </summary>
    /// <param name="collision">The player.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(collision.gameObject.transform.position.x > transform.position.x)
            {
                DeactivateObjects();
                if (ObjectToActivateRight != null)
                    ObjectToActivateRight.SetActive(true);
            }
            else
            {
                DeactivateObjects();
                if (ObjectToActivateLeft != null)
                    ObjectToActivateLeft.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Deactivates all objects.
    /// </summary>
    void DeactivateObjects()
    {
        for (int i = 0; i < ObjectsToDeactivate.Length; i++)
        {
            ObjectsToDeactivate[i].SetActive(false);
        }
    }
}
