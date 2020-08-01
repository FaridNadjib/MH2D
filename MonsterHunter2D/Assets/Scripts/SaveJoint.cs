using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveJoint : MonoBehaviour
{
    HingeJoint2D joint;
    static List<SaveJoint> AllSavedJoints = new List<SaveJoint>();


    private void Awake()
    {
        AllSavedJoints.Add(this);
        joint = GetComponent<HingeJoint2D>();
    }
   

    public static void ReassignJoints()
    {
        foreach (var item in AllSavedJoints)
        {
            item.ReassignJoint();
        }
    }

    public void ReassignJoint()
    {
        HingeJoint2D tmpJoint = gameObject.AddComponent<HingeJoint2D>();
        //TODO
        tmpJoint.anchor = joint.anchor;
    }

}
