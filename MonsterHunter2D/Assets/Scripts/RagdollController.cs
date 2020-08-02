using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{

    [SerializeField]GameObject[] bodyParts;

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
