﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelItemHolder : MonoBehaviour
{
    [SerializeField] LevelItems itemsToSave;

    [SerializeField] LevelStats levelStats;

    int currentSecrets;
    int maxSecrets;
    public LevelItems ItemsToSave { get => itemsToSave; private set => itemsToSave = value; }
    public int CurrentSecrets { get => currentSecrets; private set => currentSecrets = value; }
    public int MaxSecrets { get => maxSecrets; private set => maxSecrets = value; }


    private void Start()
    {
        Debug.Log("Loaded from start levelitemholder");
        LoadItemStatus();
    }

    public void SaveItemStatus()
    {
        
        for (int i = 0; i < 3; i++)
        {
            if(i == 0)
            {
                for (int j = 0; j < itemsToSave.pickUpItems.Length; j++)
                {
                    if (itemsToSave.pickUpItems[j] == null)
                    {
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", false);
                    }
                    else
                    {
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", itemsToSave.pickUpItems[j].activeSelf);
                    }
                }
            }

            if (i == 1)
            {
                for (int j = 0; j < itemsToSave.storyItems.Length; j++)
                {
                    if (itemsToSave.storyItems[j] == null)
                    {
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", false);
                    }
                    else
                    {
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", itemsToSave.storyItems[j].activeSelf);
                    }
                }
            }

            if (i == 2)
            {
                for (int j = 0; j < itemsToSave.miscellaneousItems.Length; j++)
                {
                    if (itemsToSave.miscellaneousItems[j] == null)
                    {
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", false);
                    }
                    else
                    {
                        levelStats.SetItemInfo($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", itemsToSave.miscellaneousItems[j].activeSelf);
                    }
                }
            }
        }

        GetSecretRatio();
    }

    public void LoadItemStatus()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < itemsToSave.pickUpItems.Length; j++)
                {
                    if (levelStats.pickUpItemInfos.ContainsKey($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"))
                    {
                        itemsToSave.pickUpItems[j].SetActive(levelStats.pickUpItemInfos[$"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"]);
                    }
                }
            }

            if (i == 1)
            {
                for (int j = 0; j < itemsToSave.storyItems.Length; j++)
                {
                    if (levelStats.pickUpItemInfos.ContainsKey($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"))
                    {
                        itemsToSave.storyItems[j].SetActive(levelStats.pickUpItemInfos[$"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"]);
                    }
                }
            }

            if (i == 2)
            {
                for (int j = 0; j < itemsToSave.miscellaneousItems.Length; j++)
                {
                    if (levelStats.pickUpItemInfos.ContainsKey($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"))
                    {
                        itemsToSave.miscellaneousItems[j].SetActive(levelStats.pickUpItemInfos[$"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"]);
                    }
                }
            }


            //foreach (var k in levelStats.pickUpItemInfos)
            //    Debug.Log("Key: "+ k.Key + " Value: " + k.Value + ".");
        }
    }

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
