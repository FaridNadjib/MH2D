using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is attached to a GO with a trigger collider, it will mark the level as completed and loads the hubscene.
/// </summary>
public class LevelEnd : MonoBehaviour
{
    #region Fields
    [Tooltip("It will save the level in playerprefs as completed. Which level number?(not the build index)")]
    [SerializeField] int levelCompleted;
    #endregion

    /// <summary>
    /// Checks if the player enters the level end area. Then it brings him back to hub.
    /// </summary>
    /// <param name="collision">Checks for the player.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerPrefs.SetInt($"Level{levelCompleted}Finished", 1);
            GameManager.instance.BackToHub();
        }
    }
}
