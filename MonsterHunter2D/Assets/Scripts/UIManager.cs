using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Player Stats related:")]
    [SerializeField] Image healthBarFill;
    [SerializeField] TextMeshProUGUI numberOfLives;
    [SerializeField] Image staminaBarFill;
    [SerializeField] Image weapon1Icon;
    [SerializeField] Image weapon2Icon;
    [SerializeField] CharacterResources characterResources;

    public static UIManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        characterResources.OnHpChanged += (float fillAmount) => {healthBarFill.fillAmount = fillAmount;};
        characterResources.OnStaminaChanged += (float fillAmount) => { staminaBarFill.fillAmount = fillAmount; };

        PlayerWeaponChanger.instance.OnWeaponChanged += (ActiveWeaponType activeWeapon, ActiveWeaponHand activeHand, Sprite weaponIcon) => {
            if (activeHand == ActiveWeaponHand.Left)
                weapon1Icon.sprite = weaponIcon;
            else
                weapon2Icon.sprite = weaponIcon;
            // ToDo: activate highlate to show which hand is active atm.
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
