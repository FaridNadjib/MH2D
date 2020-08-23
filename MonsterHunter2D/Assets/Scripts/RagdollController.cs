using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Has an array of objects. Will activate their rigidbody and joints once the unit died to simulate a ragdoll effect.
/// </summary>
public class RagdollController : MonoBehaviour
{

    [SerializeField]GameObject[] bodyParts;

    /// <summary>
    /// Enables the ragdoll.
    /// </summary>
    public void EnableRagdoll()
    {
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].GetComponent<Rigidbody2D>().isKinematic = false;
            bodyParts[i].GetComponent<Collider2D>().enabled = true;
            if (bodyParts[i].GetComponent<HingeJoint2D>() != null)
                bodyParts[i].GetComponent<HingeJoint2D>().enabled = true;
        }
        
    }

    /// <summary>
    /// Disables the ragdoll again. Never used since we just reloaded the scene instead.
    /// </summary>
    public void DisableRagdoll()
    {
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].GetComponent<Rigidbody2D>().isKinematic = true;
            bodyParts[i].GetComponent<Collider2D>().enabled = false;
            if (bodyParts[i].GetComponent<HingeJoint2D>() != null)
                bodyParts[i].GetComponent<HingeJoint2D>().enabled = false;
        }
    }
}
