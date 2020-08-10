using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Crystals, NumberOfHearts, MaxHealth, MaxStamina, MaxInvisibilityTime, MaxArrows, MaxSpears, MaxPlatformspears, MaxBombNormal, MaxStickyBomb, MaxMegaBomb, RefillHealth };
public class ItemPickUp : MonoBehaviour
{
    [Header("What this item is:")]
    [SerializeField] PlayerStats playerStats;
    [SerializeField] ItemType itemType;
    [SerializeField] int changeAmount;
    [SerializeField] string ItemEffectDescription;
    [SerializeField] SpriteRenderer light;
    [SerializeField] GameObject pickUpEffects;   

    SpriteRenderer sprite;
    Collider2D col;
    ParticleSystem particle;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        particle = GetComponent<ParticleSystem>();
        source = GetComponent<AudioSource>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {   
            // Disable the normal sprite and enable the pick up effects.
            if(sprite != null)
                sprite.enabled = false;
            if (col != null)
                col.enabled = false;
            if (light != null)
                light.enabled = false;
            if (particle != null)
                particle.Stop();
            if (source != null)
                source.Stop();
            if(pickUpEffects != null)
                pickUpEffects.SetActive(true);

            // Set the players stats active weapon and amount to default value.
            playerStats.WeaponType = ActiveWeaponType.None;
            playerStats.Amount = 0;

            // Depending on which itemtype, add the amount to the according variable from the player stats. These value changes are permanent.
            if(itemType == ItemType.Crystals)
            {
                // Todo set what happens when 100 is reached.
                playerStats.CurrentCrystals++;
            }
            else if (itemType == ItemType.RefillHealth)
                collision.GetComponent<CharacterResources>()?.AddHealth((float)changeAmount);
            else if (itemType == ItemType.NumberOfHearts)
                playerStats.NumberOfHearts += changeAmount;
            else if (itemType == ItemType.MaxHealth)
                playerStats.MaxHealth += changeAmount;
            else if (itemType == ItemType.MaxStamina)
                playerStats.MaxStamina += changeAmount;
            else if (itemType == ItemType.MaxInvisibilityTime)
                playerStats.MaxInvisibilityTime += changeAmount;
            else if (itemType == ItemType.MaxArrows)
            {
                playerStats.MaxArrows += changeAmount;
                playerStats.WeaponType = ActiveWeaponType.ArrowNormal;
                playerStats.Amount = changeAmount;
            }
            else if (itemType == ItemType.MaxSpears)
            {
                playerStats.MaxSpears += changeAmount;
                playerStats.WeaponType = ActiveWeaponType.SpearNormal;
                playerStats.Amount = changeAmount;
            }
            else if (itemType == ItemType.MaxPlatformspears)
            {
                playerStats.MaxPlatformspears += changeAmount;
                playerStats.WeaponType = ActiveWeaponType.SpearPlatform;
                playerStats.Amount = changeAmount;
            }
            else if (itemType == ItemType.MaxBombNormal)
            {
                playerStats.MaxBombNormal += changeAmount;
                playerStats.WeaponType = ActiveWeaponType.BombNormal;
                playerStats.Amount = changeAmount;
            }
            else if (itemType == ItemType.MaxStickyBomb)
            {
                playerStats.MaxStickyBomb += changeAmount;
                playerStats.WeaponType = ActiveWeaponType.BombSticky;
                playerStats.Amount = changeAmount;
            }
            else if (itemType == ItemType.MaxMegaBomb)
            {
                playerStats.MaxMegaBomb += changeAmount;
                playerStats.WeaponType = ActiveWeaponType.BombMega;
                playerStats.Amount = changeAmount;
            }
            

            // Make the UI Manager update the text displays.
            UIManager.instance.ShowPopUpMessage(ItemEffectDescription);

            playerStats.ValuesChanged();

            Destroy(this.gameObject, 2f);
        }
    }
}
