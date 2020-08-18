using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Depending on the itemtype, values are being added to the savesystem after picking it up. Farid.
/// </summary>
public enum ItemType { Crystals, NumberOfHearts, MaxHealth, MaxStamina, MaxInvisibilityTime, MaxArrows, MaxSpears, MaxPlatformspears, MaxBombNormal, MaxStickyBomb, MaxMegaBomb, RefillHealth };
/// <summary>
/// This class handles our items that can be picked up by the player. Farid.
/// </summary>
public class ItemPickUp : MonoBehaviour
{
    #region Fields
    [Header("What this item is:")]
    [Tooltip("ScriptableObject to save the data.")]
    [SerializeField] PlayerStats playerStats;
    [Tooltip("The type of the item.")]
    [SerializeField] ItemType itemType;
    [Tooltip("How many units should be added?")]
    [SerializeField] int changeAmount;
    [Tooltip("This will get displayed by the UIMananger.")]
    [SerializeField] string ItemEffectDescription;
    [Tooltip("Some pickUp effects.")]
    [SerializeField] SpriteRenderer light;
    [Tooltip("Some pickUp effects.")]
    [SerializeField] GameObject pickUpEffects;   

    // Default components.
    SpriteRenderer sprite;
    Collider2D col;
    ParticleSystem particle;
    AudioSource source;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Get the default components.
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        particle = GetComponent<ParticleSystem>();
        source = GetComponent<AudioSource>();
    }

    /// <summary>
    /// If the player triggers with the item to pick up, it checks what it is, then adds the amount to the corresponding savestats, plays pickup effects and destroys itself.
    /// </summary>
    /// <param name="collision">Checks for the player.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {   
            // Disable the normal sprite, collider etc. and enable the pick up effects.
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
                // 100 crystals will earn the player a heart(extra life).
                playerStats.CurrentCrystals++;
                if (playerStats.CurrentCrystals >= 100)
                {
                    playerStats.CurrentCrystals -= 100;
                    playerStats.NumberOfHearts++;
                }
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

            // Due to performance, dont destroy the crystals, since there are many in the scene, start coroutine and disable them instead.
            if(itemType == ItemType.Crystals)
                StartCoroutine(DeactivateInTime(2f));
            else
                Destroy(this.gameObject, 2f);
        }
    }

    /// <summary>
    /// This coroutine is used to disable the crystals after some time.
    /// </summary>
    /// <param name="time">Time to disable the gameobject.</param>
    /// <returns></returns>
    IEnumerator DeactivateInTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

}
