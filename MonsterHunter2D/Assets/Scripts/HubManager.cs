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
    [SerializeField] GameObject sunny;
    [SerializeField] GameObject foodStock;
    [SerializeField] GameObject fan;
    [SerializeField] GameObject flowers;
    [SerializeField] GameObject childOnWomen;
    [SerializeField] GameObject illChild;
    [SerializeField] Animator child1;
    [SerializeField] Animator child2;
    [SerializeField] GameObject cry;
    [SerializeField] AudioClip sadMusic;
    [SerializeField] AudioClip happyMusic;
    [SerializeField] AudioSource source2;

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
        UpdateSecrets();

        level2.SetActive(false);
        level3.SetActive(false);
        level4.SetActive(false);
        rain.SetActive(true);
        cry.SetActive(true);

        if (PlayerPrefs.HasKey("Level1Finished") && PlayerPrefs.GetInt("Level1Finished") == 1)
        {
            level2.SetActive(true);
            storyMessage.text = "They wont stop crying Khaan. \n We need to get some food. \n Please go and get some of those delicious mumbleberrys they like so much.";         
            bonFire.SetActive(true);
        }
        else
        {
            storyMessage.text = "Brrr.. Kkk Khaan, it is so cold in here. P p please you have to get us some dry firewood.";
            level1Secrets.text = "??? Secrets";
            if (source != null)
                source.PlayOneShot(sadMusic);
        }

        if (PlayerPrefs.HasKey("Level2Finished") && PlayerPrefs.GetInt("Level2Finished") == 1)
        {
            level3.SetActive(true);
            storyMessage.text = "Oh no, something terrible has happend. Little Louisa got ill. She`s gonna die. \n Only the really rare cuthullu herb could save her. Can you find some?";
            foodStock.SetActive(true);
            fan.SetActive(true);
            rain.SetActive(false);
            sunny.SetActive(true);
            child2.SetBool("isHappy", true);
            childOnWomen.SetActive(false);
            illChild.SetActive(true);
            cry.SetActive(false);
        }

        if (PlayerPrefs.HasKey("Level3Finished") && PlayerPrefs.GetInt("Level3Finished") == 1)
        {
            level4.SetActive(true);
            storyMessage.text = "We love you so much. Please be careful when exploring the djungle.";
            flowers.SetActive(true);
            illChild.SetActive(false);
            childOnWomen.SetActive(true);
            child1.SetBool("isHappy", true);
            source.PlayOneShot(happyMusic);
        }

        if (PlayerPrefs.HasKey("Level4Finished") && PlayerPrefs.GetInt("Level4Finished") == 1)
        {
            storyMessage.text = "Khaan, you are truely the King of the djungle. \n What do you want to do today?";
        }
        // Todo change position of the women.
    }

    /// <summary>
    /// Sets the secrets text according to how many secret have been discoverd.
    /// </summary>
    private void UpdateSecrets()
    {
        level1Secrets.text = "??? Secrets";
        level2Secrets.text = "??? Secrets";
        level3Secrets.text = "??? Secrets";
        level4Secrets.text = "??? Secrets";

        if (PlayerPrefs.HasKey("Level2MaxSecrets"))
            level1Secrets.text = $"{PlayerPrefs.GetInt("Level2CurrentSecrets")} / {PlayerPrefs.GetInt("Level2MaxSecrets")}";
        if (PlayerPrefs.HasKey("Level3MaxSecrets"))
            level2Secrets.text = $"{PlayerPrefs.GetInt("Level3CurrentSecrets")} / {PlayerPrefs.GetInt("Level3MaxSecrets")}";
        if (PlayerPrefs.HasKey("Level4MaxSecrets"))
            level3Secrets.text = $"{PlayerPrefs.GetInt("Level4CurrentSecrets")} / {PlayerPrefs.GetInt("Level4MaxSecrets")}";
        if (PlayerPrefs.HasKey("Level5MaxSecrets"))
            level4Secrets.text = $"{PlayerPrefs.GetInt("Level5CurrentSecrets")} / {PlayerPrefs.GetInt("Level5MaxSecrets")}";
    }

    private void Update()
    {
        // Checks if the player can talk to the woman.
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
        source2.Play();
    }
    /// <summary>
    /// Loads the level from index. Its one more than the level since our hubScene has build index 1.
    /// </summary>
    public void StartLevel02()
    {
        GameManager.instance.LoadLevel(3);
        source2.Play();
    }
    /// <summary>
    /// Loads the level from index. Its one more than the level since our hubScene has build index 1.
    /// </summary>
    public void StartLevel03()
    {
        GameManager.instance.LoadLevel(4);
        source2.Play();
    }
    /// <summary>
    /// Loads the level from index. Its one more than the level since our hubScene has build index 1.
    /// </summary>
    public void StartLevel04()
    {
        GameManager.instance.LoadLevel(5);
        source2.Play();
    }
}
