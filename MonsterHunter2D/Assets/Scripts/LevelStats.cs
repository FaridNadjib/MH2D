using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object where we can save what the player actually has picked up into a dictionary.
/// </summary>
[CreateAssetMenu(menuName = "LevelStats")]
public class LevelStats : ScriptableObject
{
    public Dictionary<string, bool> pickUpItemInfos = new Dictionary<string, bool>();

    /// <summary>
    /// Set a key and a corresponding value representing the item we want to save and its activeself status.
    /// </summary>
    /// <param name="index">The Key.</param>
    /// <param name="active">The activeSelf status.</param>
    public void SetItemInfo(string index, bool active)
    {
        if (!pickUpItemInfos.ContainsKey(index))
            pickUpItemInfos.Add(index, active);
        else
            pickUpItemInfos[index] = active;
    }

    /// <summary>
    /// Clears all saved data.
    /// </summary>
    public void ClearSavedData()
    {
        pickUpItemInfos.Clear();
    }
}
