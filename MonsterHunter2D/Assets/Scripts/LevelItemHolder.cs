using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class saves the activeself status of every item we want to save. Based on that playerprefs are set in the gameManager.
/// </summary>
public class LevelItemHolder : MonoBehaviour
{
    #region Fields and Properties
    [Tooltip("What items we want to save?")]
    [SerializeField] LevelItems itemsToSave;
    [Tooltip("The scriptable object.")]
    [SerializeField] LevelStats levelStats;

    int currentSecrets;
    int maxSecrets;
    public LevelItems ItemsToSave { get => itemsToSave; private set => itemsToSave = value; }
    public int CurrentSecrets { get => currentSecrets; private set => currentSecrets = value; }
    public int MaxSecrets { get => maxSecrets; private set => maxSecrets = value; }
    #endregion

    private void Start()
    {
        LoadItemStatus();
    }

    /// <summary>
    /// For every gameObject in the items to save array generate a key and save the activeSelf into the playerprefs. Destroyed or deactivated objects will get deactivated if you load the scene again and their status has been saved.
    /// </summary>
    public void SaveItemStatus()
    {     
        for (int i = 0; i < 3; i++)
        {
            if(i == 0)
            {
                for (int j = 0; j < itemsToSave.pickUpItems.Length; j++)
                {
                    if (itemsToSave.pickUpItems[j] == null)
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", false);
                    else
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", itemsToSave.pickUpItems[j].activeSelf);
                }
            }

            if (i == 1)
            {
                for (int j = 0; j < itemsToSave.storyItems.Length; j++)
                {
                    if (itemsToSave.storyItems[j] == null)
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", false);
                    else
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", itemsToSave.storyItems[j].activeSelf);
                }
            }

            if (i == 2)
            {
                for (int j = 0; j < itemsToSave.miscellaneousItems.Length; j++)
                {
                    if (itemsToSave.miscellaneousItems[j] == null)
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", false);
                    else
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", itemsToSave.miscellaneousItems[j].activeSelf);
                }
            }
        }

        GetSecretRatio();
    }

    /// <summary>
    /// Set the active status of the items in items to save array depending on the stat we save into playerprefs, if there is no key the object is active.
    /// </summary>
    public void LoadItemStatus()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < itemsToSave.pickUpItems.Length; j++)
                {
                    if (levelStats.pickUpItemInfos.ContainsKey($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"))
                        itemsToSave.pickUpItems[j].SetActive(levelStats.pickUpItemInfos[$"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"]);
                }
            }

            if (i == 1)
            {
                for (int j = 0; j < itemsToSave.storyItems.Length; j++)
                {
                    if (levelStats.pickUpItemInfos.ContainsKey($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"))
                        itemsToSave.storyItems[j].SetActive(levelStats.pickUpItemInfos[$"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"]);
                }
            }

            if (i == 2)
            {
                for (int j = 0; j < itemsToSave.miscellaneousItems.Length; j++)
                {
                    if (levelStats.pickUpItemInfos.ContainsKey($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"))
                        itemsToSave.miscellaneousItems[j].SetActive(levelStats.pickUpItemInfos[$"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"]);
                }
            }
        }
    }

    /// <summary>
    /// The length of pickUpItems from items to save is our max secrets number, based on deactivaed objects in that array we know how many items the player already collected.
    /// </summary>
    void GetSecretRatio()
    {
        CurrentSecrets = 0;
        MaxSecrets = itemsToSave.pickUpItems.Length;
        for (int i = 0; i < itemsToSave.pickUpItems.Length; i++)
        {
            if (itemsToSave.pickUpItems[i] == null || itemsToSave.pickUpItems[i].activeSelf == false)
                CurrentSecrets++;
        }
    }
}
