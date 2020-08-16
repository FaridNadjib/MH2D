using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LevelStats")]
public class LevelStats : ScriptableObject
{
    public Dictionary<string, bool> pickUpItemInfos = new Dictionary<string, bool>();

    public void SetItemInfo(string index, bool active)
    {
        if (!pickUpItemInfos.ContainsKey(index))
            pickUpItemInfos.Add(index, active);
        else
            pickUpItemInfos[index] = active;
    }

    //todo clear method

    public void ClearSavedData()
    {
        pickUpItemInfos.Clear();
    }
}
