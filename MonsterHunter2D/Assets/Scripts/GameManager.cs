using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

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


    // Start is called before the first frame update
    void Start()
    {
        // Try to grab the itemloader, since Gamemanager is Singleton, this will never get used i think. ToDo: delete the try catch.
        try
        {
            itemLoader = GameObject.Find("LevelItemLoader").GetComponent<LevelItemHolder>();
        }
        catch (System.Exception)
        {
            itemLoader = null;
        }

        // Subscribe to this unity event. Everytime a scene is changed, it gets triggerd.
        SceneManager.activeSceneChanged += ChangedActiveScene;

        // Change the cursor.
        if(cursor != null)
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.N))
            ResetGameStats();


        if (Input.GetKeyDown(KeyCode.V))
            LoadLevel(0);
        if (Input.GetKeyDown(KeyCode.C))
            LoadLevel(1);
        if (Input.GetKeyDown(KeyCode.X))
            LoadLevel(2);



        if (Input.GetKeyDown(KeyCode.L))
            LoadStats();

        if (Input.GetKeyDown(KeyCode.H))
            SaveStats();

        // Activate/deactivate the ingame menu.
        if (UIManager.instance != null && Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
                UIManager.instance.ShowHubMenu();
            if (SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 0)
                UIManager.instance.ShowRetryMenu();
        }
            
    }

    /// <summary>
    /// Resets all progress and starts a new fresh game.
    /// </summary>
    public void StartNewGame()
    {
        ResetGameStats();
        LoadLevel(1);
    }

    /// <summary>
    /// Load saved game stats and then load the hub level.
    /// </summary>
    public void ContinueGame()
    {
        LoadStats();
        LoadLevel(1);
    }

    public void RetryLevel()
    {
        if (itemLoader != null)
            itemLoader.SaveItemStatus();

        LoadLevel(SceneManager.GetActiveScene().buildIndex);
        
    }

    public void BackToHub()
    {
        if (itemLoader != null)
            itemLoader.SaveItemStatus();

        LoadLevel(1);
    }

    public void SaveAndExit()
    {
        if (itemLoader != null)
            itemLoader.SaveItemStatus();

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


    public void SaveStats()
    {
        PlayerPrefs.SetFloat("NumberOfHearts", playerStats.NumberOfHearts);

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

    public void LoadStats()
    {
        if (PlayerPrefs.HasKey("NumberOfHearts"))
        {
            playerStats.NumberOfHearts = (int)PlayerPrefs.GetFloat("NumberOfHearts");
        }

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

    public void ResetGameStats()
    {
        PlayerPrefs.DeleteAll();
        levelStats.ClearSavedData();

        playerStats.ResetPlayerStats();
        PlayerPrefs.SetFloat("NumberOfHearts", playerStats.NumberOfHearts);

        if (itemLoader != null)
        {
            //for (int i = 0; i < 3; i++)
            //{
            //    if(i == 0)
            //    {
            //        for (int j = 0; j < itemLoader.ItemsToSave.pickUpItems.Length; j++)
            //        {

            //            if (levelStats.pickUpItemInfos.ContainsKey($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"))
            //            {
            //                PlayerPrefs.SetInt($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", Convert.ToInt32(levelStats.pickUpItemInfos[$"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"]));
            //                levelStats.pickUpItemInfos[$"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"] = true;
            //                Debug.Log("Key renewed");
            //            }
            //            else
            //            {
            //                levelStats.pickUpItemInfos.Add($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", true);
                            
            //                PlayerPrefs.SetInt($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}", 1);
            //                Debug.Log("Playerpref here: " + PlayerPrefs.GetInt($"_{SceneManager.GetActiveScene().buildIndex}{i}{j}"));
            //            }
            //        }
            //    }
            //}
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
        else if(UIManager.instance != null)
            UIManager.instance.gameObject.SetActive(true);

        // Get the new LevelItemLoader of the new Scene.
        if (next.buildIndex == SceneManager.GetActiveScene().buildIndex)
        {
            try { itemLoader = GameObject.Find("LevelItemLoader").GetComponent<LevelItemHolder>(); }
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
