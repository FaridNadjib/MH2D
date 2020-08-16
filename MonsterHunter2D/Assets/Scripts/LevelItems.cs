using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// This class is used by the LevelItemHolder class to create an array out of the arrays of this class.
/// </summary>
[Serializable]
public class LevelItems
{
    public GameObject[] pickUpItems;
    public GameObject[] storyItems;
    public GameObject[] miscellaneousItems;
}
