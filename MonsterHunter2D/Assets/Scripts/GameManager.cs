using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// This class is our gamemanager, it handles the scene loading and the loading and saving in general. Farid.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    static public GameManager instance;
    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);       
    }
    #endregion

    #region Fields & Properties
    [Header("For saving our Data:")]
    [SerializeField] PlayerStats playerStats;
    [SerializeField] LevelStats levelStats;
    // In order to save, scenes have a have GO called "LevelItemLoader, where all items to save are stored in an array.
    LevelItemHolder itemLoader;

    [Header ("Custom cursor:")]
    [SerializeField] Texture2D cursor;

    [Header("Leveltransitions:")]
    [SerializeField] Animator transitionAnim;
    [SerializeField] float transitionTime;

    public bool GameOver { get; set; } = false;
    #endregion

    #region Special PlayerPref Keys
    // HasSaveData, int. 0 or Key not there means no save data.
    // LevelXFinished, int 0 means not finished 1 means was finished. X is the number of our level(not the scene index)
    // Level{buildIndex}MaxSecrets and Level{buildIndex}CurrentSecrets save the secret count from the corresponding level.
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        // Try to grab the itemloader.
        try
        {
            itemLoader = GameObject.Find("LevelItemLoader").GetComponent<LevelItemHolder>();
        }
        catch (System.Exception) { itemLoader = null; }


        // Subscribe to this unity event. Everytime a scene is changed, it gets triggerd.
        SceneManager.activeSceneChanged += ChangedActiveScene;

        // Change the cursor.
        if(cursor != null)
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
    }


    // Update is called once per frame
    void Update()
    {
        // Activate/deactivate the ingame menu.
        if (UIManager.instance != null && Input.GetKeyDown(KeyCode.Escape) && !GameOver)
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
                UIManager.instance.ShowHubMenu();
            if (SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 0)
                UIManager.instance.ShowRetryMenu();
        }

        if (GameOver)
        {
            UIManager.instance.ShowGameOverScreen(true);
            ClearSavedData();
            StartCoroutine(GameLost());
            GameOver = false;
        }
    }

    /// <summary>
    /// The game is lost, wait and send the player back to main menu.
    /// </summary>
    /// <returns></returns>
    IEnumerator GameLost()
    {
        yield return new WaitForSeconds(2f);
        if (transitionAnim != null)
            transitionAnim.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Deletes all saved data.
    /// </summary>
    void ClearSavedData()
    {
        PlayerPrefs.DeleteAll();
        levelStats.ClearSavedData();
        playerStats.DeleteSpawnPos();
    }


    /// <summary>
    /// Resets all progress and starts a new fresh game.
    /// </summary>
    public void StartNewGame()
    {
        playerStats.DeleteSpawnPos();
        ResetGameStats();
        LoadLevel(1);
    }

    /// <summary>
    /// Load saved game stats and then load the hub level.
    /// </summary>
    public void ContinueGame()
    {
        playerStats.DeleteSpawnPos();
        LoadStats();
        LoadLevel(1);
    }

    /// <summary>
    /// Saves the stats and reloads the current level.
    /// </summary>
    public void RetryLevel()
    {
        if (itemLoader != null)
            itemLoader.SaveItemStatus();
        SaveStats();
        LoadLevel(SceneManager.GetActiveScene().buildIndex);   
    }

    /// <summary>
    /// Lets the player go back from level to the hubscene.
    /// </summary>
    public void BackToHub()
    {
        if (itemLoader != null)
            itemLoader.SaveItemStatus();
        playerStats.DeleteSpawnPos();
        SaveStats();
        LoadLevel(1);
    }

    /// <summary>
    /// Saves the stats and returns to mainmenu.
    /// </summary>
    public void SaveAndExit()
    {
        if (itemLoader != null)
            itemLoader.SaveItemStatus();
        SaveStats();
        LoadLevel(0);
    }

    /// <summary>
    /// Save the itemstatus and then load the scene from index.
    /// </summary>
    /// <param name="index">The scene to load.</param>
    public void LoadLevel(int index)
    {
        // Save the itemstatus when you leave a scene.
        if (itemLoader != null)
            itemLoader.SaveItemStatus();

        StartCoroutine(LoadLevelTransition(index));
    }

    /// <summary>
    /// Load the scene after a transition delay.
    /// </summary>
    /// <param name="levelIndex">The scene to load.</param>
    /// <returns></returns>
    IEnumerator LoadLevelTransition(int levelIndex)
    {
        if(transitionAnim != null)
            transitionAnim.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }

    /// <summary>
    /// Saves all the relevant data to playerprefs.
    /// </summary>
    public void SaveStats()
    {
        PlayerPrefs.SetInt("CurrentCrystals", playerStats.CurrentCrystals);
        PlayerPrefs.SetInt("NumberOfHearts", playerStats.NumberOfHearts);
        PlayerPrefs.SetInt("CurrentNumberOfHearts", playerStats.CurrentNumberOfHearts);
        PlayerPrefs.SetFloat("MaxHealth", playerStats.MaxHealth);
        PlayerPrefs.SetFloat("MaxStamina", playerStats.MaxStamina);
        PlayerPrefs.SetFloat("MaxInvisibilityTime", playerStats.MaxInvisibilityTime);
        PlayerPrefs.SetInt("MaxArrows", playerStats.MaxArrows);
        PlayerPrefs.SetInt("MaxSpears", playerStats.MaxSpears);
        PlayerPrefs.SetInt("MaxPlatformspears", playerStats.MaxPlatformspears);
        PlayerPrefs.SetInt("MaxBombNormal", playerStats.MaxBombNormal);
        PlayerPrefs.SetInt("MaxStickyBomb", playerStats.MaxStickyBomb);
        PlayerPrefs.SetInt("MaxMegaBomb", playerStats.MaxMegaBomb);

        PlayerPrefs.SetInt("HasSaveData", 1);

        // Get current secrets unlocked by the player.
        PlayerPrefs.SetInt($"Level{SceneManager.GetActiveScene().buildIndex}MaxSecrets", itemLoader.MaxSecrets);
        PlayerPrefs.SetInt($"Level{SceneManager.GetActiveScene().buildIndex}CurrentSecrets", itemLoader.CurrentSecrets);

        int activeScene = SceneManager.GetActiveScene().buildIndex;
        if (itemLoader != null)
        {
            itemLoader.SaveItemStatus();

            for (int i = 0; i < 3; i++)
            {
                if(i == 0)
                {
                    for (int j = 0; j < itemLoader.ItemsToSave.pickUpItems.Length; j++)
                    {
                        if (levelStats.pickUpItemInfos.ContainsKey($"_{activeScene}{i}{j}"))
                        {
                            PlayerPrefs.SetInt($"_{activeScene}{i}{j}", Convert.ToInt32(levelStats.pickUpItemInfos[$"_{activeScene}{i}{j}"]));
                            Debug.Log("Key saved: "  + levelStats.pickUpItemInfos[$"_{activeScene}{i}{j}"]);
                        }
                        else
                        {
                            //should never gets called, 
                            //levelStats.SetItemInfo($"_{activeScene}{i}{j}", Convert.ToBoolean(PlayerPrefs.GetInt($"_{activeScene}{i}{j}")));
                        }
                    }
                }
                if (i == 1)
                {
                    for (int j = 0; j < itemLoader.ItemsToSave.storyItems.Length; j++)
                    {
                        if (levelStats.pickUpItemInfos.ContainsKey($"_{activeScene}{i}{j}"))
                        {
                            PlayerPrefs.SetInt($"_{activeScene}{i}{j}", Convert.ToInt32(levelStats.pickUpItemInfos[$"_{activeScene}{i}{j}"]));
                            Debug.Log("Key saved: " + levelStats.pickUpItemInfos[$"_{activeScene}{i}{j}"]);
                        }
                    }
                }
                if (i == 2)
                {
                    for (int j = 0; j < itemLoader.ItemsToSave.miscellaneousItems.Length; j++)
                    {
                        if (levelStats.pickUpItemInfos.ContainsKey($"_{activeScene}{i}{j}"))
                        {
                            PlayerPrefs.SetInt($"_{activeScene}{i}{j}", Convert.ToInt32(levelStats.pickUpItemInfos[$"_{activeScene}{i}{j}"]));
                            Debug.Log("Key saved: " + levelStats.pickUpItemInfos[$"_{activeScene}{i}{j}"]);
                        }
                    }
                }
            }
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Loads all the relevant data from playerprefs.
    /// </summary>
    public void LoadStats()
    {
        if (PlayerPrefs.HasKey("CurrentCrystals")) { playerStats.CurrentCrystals = PlayerPrefs.GetInt("CurrentCrystals"); }
        if (PlayerPrefs.HasKey("NumberOfHearts")) { playerStats.NumberOfHearts = PlayerPrefs.GetInt("NumberOfHearts"); }
        if (PlayerPrefs.HasKey("CurrentNumberOfHearts")) { playerStats.CurrentNumberOfHearts = PlayerPrefs.GetInt("CurrentNumberOfHearts"); }
        if (PlayerPrefs.HasKey("MaxHealth")) { playerStats.MaxHealth = PlayerPrefs.GetFloat("MaxHealth"); }
        if (PlayerPrefs.HasKey("MaxStamina")) { playerStats.MaxStamina = PlayerPrefs.GetFloat("MaxStamina"); }
        if (PlayerPrefs.HasKey("MaxInvisibilityTime")) { playerStats.MaxInvisibilityTime = PlayerPrefs.GetFloat("MaxInvisibilityTime"); }
        if (PlayerPrefs.HasKey("MaxArrows")) { playerStats.MaxArrows = PlayerPrefs.GetInt("MaxArrows"); }
        if (PlayerPrefs.HasKey("MaxSpears")) { playerStats.MaxSpears = PlayerPrefs.GetInt("MaxSpears"); }
        if (PlayerPrefs.HasKey("MaxPlatformspears")) { playerStats.MaxPlatformspears = PlayerPrefs.GetInt("MaxPlatformspears"); }
        if (PlayerPrefs.HasKey("MaxBombNormal")) { playerStats.MaxBombNormal = PlayerPrefs.GetInt("MaxBombNormal"); }
        if (PlayerPrefs.HasKey("MaxStickyBomb")) { playerStats.MaxStickyBomb = PlayerPrefs.GetInt("MaxStickyBomb"); }
        if (PlayerPrefs.HasKey("MaxMegaBomb")) { playerStats.MaxMegaBomb = PlayerPrefs.GetInt("MaxMegaBomb"); }

        int activeScene = SceneManager.GetActiveScene().buildIndex;

        if(itemLoader != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if(i == 0)
                {
                    for (int j = 0; j < itemLoader.ItemsToSave.pickUpItems.Length; j++)
                    {
                        //Debug.Log("Saved pref data:" + PlayerPrefs.GetInt($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"));
                        //Debug.Log("Do i conttain key?:" + levelStats.pickUpItemInfos.ContainsKey($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"));
                        if (levelStats.pickUpItemInfos.ContainsKey($"_{activeScene}{i}{j}") && PlayerPrefs.HasKey($"_{activeScene}{i}{j}"))
                        {
                            levelStats.pickUpItemInfos[$"_{activeScene}{i}{j}"] = Convert.ToBoolean(PlayerPrefs.GetInt($"_{activeScene}{i}{j}"));
                        }
                        else if(PlayerPrefs.HasKey($"_{activeScene}{i}{j}"))
                        {
                            levelStats.SetItemInfo($"_{activeScene}{i}{j}", Convert.ToBoolean(PlayerPrefs.GetInt($"_{activeScene}{i}{j}")));
                        }
                    }
                }
                if (i == 1)
                {
                    for (int j = 0; j < itemLoader.ItemsToSave.storyItems.Length; j++)
                    {
                        if (levelStats.pickUpItemInfos.ContainsKey($"_{activeScene}{i}{j}") && PlayerPrefs.HasKey($"_{activeScene}{i}{j}"))
                        {
                            levelStats.pickUpItemInfos[$"_{activeScene}{i}{j}"] = Convert.ToBoolean(PlayerPrefs.GetInt($"_{activeScene}{i}{j}"));
                        }
                        else if (PlayerPrefs.HasKey($"_{activeScene}{i}{j}"))
                        {
                            levelStats.SetItemInfo($"_{activeScene}{i}{j}", Convert.ToBoolean(PlayerPrefs.GetInt($"_{activeScene}{i}{j}")));
                        }
                    }
                }
                if (i == 2)
                {
                    for (int j = 0; j < itemLoader.ItemsToSave.miscellaneousItems.Length; j++)
                    {
                        if (levelStats.pickUpItemInfos.ContainsKey($"_{activeScene}{i}{j}") && PlayerPrefs.HasKey($"_{activeScene}{i}{j}"))
                        {
                            levelStats.pickUpItemInfos[$"_{activeScene}{i}{j}"] = Convert.ToBoolean(PlayerPrefs.GetInt($"_{activeScene}{i}{j}"));
                        }
                        else if (PlayerPrefs.HasKey($"_{activeScene}{i}{j}"))
                        {
                            levelStats.SetItemInfo($"_{activeScene}{i}{j}", Convert.ToBoolean(PlayerPrefs.GetInt($"_{activeScene}{i}{j}")));
                        }
                    }
                }
            }
            //foreach (var k in levelStats.pickUpItemInfos)
            //    Debug.Log("GM Key: " + k.Key + "GM Value: " + k.Value + ".");
            itemLoader.LoadItemStatus();
        }
        playerStats.ValuesChanged();
    }

    /// <summary>
    /// Resets the game data, first it clears all saved stuff.
    /// </summary>
    public void ResetGameStats()
    {
        PlayerPrefs.DeleteAll();
        levelStats.ClearSavedData();

        playerStats.ResetPlayerStats();
        PlayerPrefs.SetInt("CurrentCrystals", playerStats.CurrentCrystals);
        PlayerPrefs.SetInt("NumberOfHearts", playerStats.NumberOfHearts);
        PlayerPrefs.SetInt("CurrentNumberOfHearts", playerStats.CurrentNumberOfHearts);
        PlayerPrefs.SetFloat("MaxHealth", playerStats.MaxHealth);
        PlayerPrefs.SetFloat("MaxStamina", playerStats.MaxStamina);
        PlayerPrefs.SetFloat("MaxInvisibilityTime", playerStats.MaxInvisibilityTime);
        PlayerPrefs.SetInt("MaxArrows", playerStats.MaxArrows);
        PlayerPrefs.SetInt("MaxSpears", playerStats.MaxSpears);
        PlayerPrefs.SetInt("MaxPlatformspears", playerStats.MaxPlatformspears);
        PlayerPrefs.SetInt("MaxBombNormal", playerStats.MaxBombNormal);
        PlayerPrefs.SetInt("MaxStickyBomb", playerStats.MaxStickyBomb);
        PlayerPrefs.SetInt("MaxMegaBomb", playerStats.MaxMegaBomb);

        PlayerPrefs.SetInt("HasSaveData", 0);
        PlayerPrefs.SetInt("Level1Finished", 0);
        PlayerPrefs.SetInt("Level2Finished", 0);
        PlayerPrefs.SetInt("Level3Finished", 0);
        PlayerPrefs.SetInt("Level4Finished", 0);
        PlayerPrefs.SetInt("Level5Finished", 0);

        if (itemLoader != null)
        {
            itemLoader.LoadItemStatus();
            PlayerPrefs.Save();
        }
    }
    
    /// <summary>
    /// This method gets called once everytime a scene has changed. Play transition and get needed variables like the LevelItemLoader.
    /// </summary>
    /// <param name="current">CurrentScene</param>
    /// <param name="next">NextScene</param>
    private void ChangedActiveScene(Scene current, Scene next)
    {
        // Play the scene transition.
        if (transitionAnim != null)
            transitionAnim.SetTrigger("End");

        // Deactivate the UI Manager in the MainmenuScene.
        if (SceneManager.GetActiveScene().buildIndex == 0 && UIManager.instance != null)
            UIManager.instance.gameObject.SetActive(false);
        else if (UIManager.instance != null)
        {
            UIManager.instance.GetPlayer();
            UIManager.instance.UpdateUI();
            UIManager.instance.HideEscapeMenu();
            UIManager.instance.ShowGameOverScreen(false);
            UIManager.instance.gameObject.SetActive(true);
        }

        // Get the new LevelItemLoader of the new Scene.
        if (next.buildIndex == SceneManager.GetActiveScene().buildIndex)
        {
            try { itemLoader = GameObject.Find("LevelItemLoader").GetComponent<LevelItemHolder>();
                if (itemLoader != null)
                    LoadStats();
            }
            catch (System.Exception) { itemLoader = null; }
        }
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                StartCoroutine(EndGameTransition());
#endif
    }

    /// <summary>
    /// Wait for the transitiontime to end before quitting the application.
    /// </summary>
    /// <returns></returns>
    IEnumerator EndGameTransition()
    {
        if (transitionAnim != null)
            transitionAnim.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        Application.Quit();
    }
}
