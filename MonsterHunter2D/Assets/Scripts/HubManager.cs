using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This class will handle the hub progression and the hub menu when talking to the women.
/// </summary>
public class HubManager : MonoBehaviour
{
    #region Fields
    [SerializeField] GameObject levelSelection;
    [SerializeField] TextMeshProUGUI storyMessage;
    [SerializeField] TextMeshProUGUI controlMessage;
    [SerializeField] TextMeshProUGUI level1Secrets;
    [SerializeField] TextMeshProUGUI level2Secrets;
    [SerializeField] TextMeshProUGUI level3Secrets;
    [SerializeField] TextMeshProUGUI level4Secrets;
    [SerializeField] GameObject level2;
    [SerializeField] GameObject level3;
    [SerializeField] GameObject level4;
    [Header("Story related object, theyll get activated once a level is done:")]
    [SerializeField] GameObject bonFire;
    [SerializeField] GameObject rain;
    [SerializeField] GameObject foodStock;
    [SerializeField] GameObject childOnWomen;
    [SerializeField] GameObject illChild;
    [SerializeField] GameObject happyChild1;
    [SerializeField] GameObject happyChild2;
    [SerializeField] AudioClip sadMusic;
    [SerializeField] AudioClip happyMusic;

    bool talkArea = false;

    AudioSource source;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();

        UpdateLevelSelectionPanel();
    }

    /// <summary>
    /// This method handles the button displayed and the story messages in the hub. Furthermore it activates and deactivates gameobjects based on story progression.
    /// </summary>
    void UpdateLevelSelectionPanel()
    {
        level2.SetActive(false);
        level3.SetActive(false);
        level4.SetActive(false);

        if (PlayerPrefs.HasKey("Level4Finished") && PlayerPrefs.GetInt("Level4Finished") == 1)
        {
            storyMessage.text = "Khaan, you are truely the King of the djungle. \n What do you want to do today?";
            level2.SetActive(true);
            level3.SetActive(true);
            level4.SetActive(true);
        }
        else if (PlayerPrefs.HasKey("Level3Finished") && PlayerPrefs.GetInt("Level3Finished") == 1)
        {
            level2.SetActive(true);
            level3.SetActive(true);
            level4.SetActive(true);
            storyMessage.text = "We love you so much. Please be careful when exploring the djungle.";
        }
        else if (PlayerPrefs.HasKey("Level2Finished") && PlayerPrefs.GetInt("Level2Finished") == 1)
        {
            level2.SetActive(true);
            level3.SetActive(true);
            storyMessage.text = "Oh no, something terrible has happend. Little Louisa got ill. She`s gonna die. \n Only the really rare cuthullu herb could save her. Can you find some?";
        }
        else if (PlayerPrefs.HasKey("Level1Finished") && PlayerPrefs.GetInt("Level1Finished") == 1)
        {
            level2.SetActive(true);
            storyMessage.text = "They wont stop crying Khaan. \n We need to get some food. \n Please go and get some of those delicious mumbleberrys they like so much.";
        }
        else
        {
            storyMessage.text = "Brrr.. Kkk Khaan, it is so cold in here. P p please you have to get us some dry firewood.";
            if (source != null)
                source.PlayOneShot(sadMusic);
        }
        //Todo activate the story objects.
        // todo change position of the women.
    }

    private void Update()
    {
        if(talkArea)
            if (Input.GetKeyDown(KeyCode.E))
                levelSelection.SetActive(true);
    }

    /// <summary>
    /// Checks if the player is in talk area and opens levelselection menu on keypress.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            controlMessage.text = "press e to talk";
            talkArea = true;
        }
    }

    /// <summary>
    /// Checks if the player leaves the talk area of the women.
    /// </summary>
    /// <param name="collision">Checks for the player.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            controlMessage.text = "";
            levelSelection.SetActive(false);
            talkArea = false;
        }
    }

    /// <summary>
    /// Loads the level from index. Its one more than the level since our hubScene has build index 1.
    /// </summary>
    public void StartLevel01()
    {
        GameManager.instance.LoadLevel(2);
    }
    /// <summary>
    /// Loads the level from index. Its one more than the level since our hubScene has build index 1.
    /// </summary>
    public void StartLevel02()
    {
        GameManager.instance.LoadLevel(3);
    }
    /// <summary>
    /// Loads the level from index. Its one more than the level since our hubScene has build index 1.
    /// </summary>
    public void StartLevel03()
    {
        GameManager.instance.LoadLevel(4);
    }
    /// <summary>
    /// Loads the level from index. Its one more than the level since our hubScene has build index 1.
    /// </summary>
    public void StartLevel04()
    {
        GameManager.instance.LoadLevel(5);
    }
}
