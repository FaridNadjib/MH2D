using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This enum represents all the different munition types the player can equip.
/// </summary>
public enum ActiveWeaponType { ArrowNormal, ArrowRope, SpearNormal, SpearPlatform, BombNormal, BombSticky, BombMega, None }
/// <summary>
/// Describes which weaponhand is active.
/// </summary>
public enum ActiveWeaponHand { Left, Right1, Right2 }

/// <summary>
/// This class changes the active weapon type by player selection.
/// </summary>
public class PlayerWeaponChanger : MonoBehaviour
{
    // An event that will get triggerd, everytime the player changed his weapon. An event the UI manager will use to get the right weapon icon to display.
    public event ActiveWeapon OnWeaponChanged;
    public delegate void ActiveWeapon(ActiveWeaponType activeWeapon, ActiveWeaponHand activeHand, Sprite activeWeaponIcon);

    ActiveWeaponType activeWeapon;
    ActiveWeaponHand activeHand;
    Sprite activeIcon;

    Dictionary<string, Queue<ActiveWeaponType>> weaponKeysDictionary = new Dictionary<string, Queue<ActiveWeaponType>>();
    Queue<ActiveWeaponType> alpha1Weapons = new Queue<ActiveWeaponType>();
    Queue<ActiveWeaponType> alpha2Weapons = new Queue<ActiveWeaponType>();
    Queue<ActiveWeaponType> alpha3Weapons = new Queue<ActiveWeaponType>();

    // This struct plus array saves all the icons corresponding to each weapontype.
    [Serializable]
    public struct WeaponIcons
    {
        public string name;
        public Sprite image;
    }
    public WeaponIcons[] weaponIcons;

    #region Singleton
    static public PlayerWeaponChanger instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    private void Start()
    {
        weaponKeysDictionary.Add("alpha1Weapons", alpha1Weapons);
        weaponKeysDictionary.Add("alpha2Weapons", alpha2Weapons);
        weaponKeysDictionary.Add("alpha3Weapons", alpha3Weapons);

        alpha1Weapons.Enqueue(ActiveWeaponType.ArrowNormal);
        //alpha1Weapons.Enqueue(ActiveWeaponType.ArrowRope);
        alpha2Weapons.Enqueue(ActiveWeaponType.SpearNormal);
        alpha2Weapons.Enqueue(ActiveWeaponType.SpearPlatform);
        alpha3Weapons.Enqueue(ActiveWeaponType.BombNormal);
        alpha3Weapons.Enqueue(ActiveWeaponType.BombSticky);
        alpha3Weapons.Enqueue(ActiveWeaponType.BombMega);


    }


    // Update is called once per frame
    void Update()
    {
        // Change weapontype. Depending on which key is pressed iterate through the weapons that can be equipped with that key.
        if (Input.GetKeyUp(KeyCode.Alpha1) && alpha1Weapons.Count != 0)
        {
            if(activeHand == ActiveWeaponHand.Left)
            {
                activeWeapon = alpha1Weapons.Dequeue();
                alpha1Weapons.Enqueue(activeWeapon);
                activeWeapon = alpha1Weapons.Peek();
            }else
            {
                activeHand = ActiveWeaponHand.Left;
                activeWeapon = alpha1Weapons.Peek();
            }
            activeIcon = GetIcon(activeWeapon.ToString());
            OnWeaponChanged?.Invoke(activeWeapon, activeHand, activeIcon);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2) && alpha2Weapons.Count != 0)
        {
            if (activeHand == ActiveWeaponHand.Right1)
            {
                activeWeapon = alpha2Weapons.Dequeue();
                alpha2Weapons.Enqueue(activeWeapon);
                activeWeapon = alpha2Weapons.Peek();
            }
            else
            {
                activeHand = ActiveWeaponHand.Right1;
                activeWeapon = alpha2Weapons.Peek();
            }
            activeIcon = GetIcon(activeWeapon.ToString());
            OnWeaponChanged?.Invoke(activeWeapon, activeHand, activeIcon);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3) && alpha3Weapons.Count != 0)
        {
            if (activeHand == ActiveWeaponHand.Right2)
            {
                activeWeapon = alpha3Weapons.Dequeue();
                alpha3Weapons.Enqueue(activeWeapon);
                activeWeapon = alpha3Weapons.Peek();
            }
            else
            {
                activeHand = ActiveWeaponHand.Right2;
                activeWeapon = alpha3Weapons.Peek();
            }
            activeIcon = GetIcon(activeWeapon.ToString());
            OnWeaponChanged?.Invoke(activeWeapon, activeHand, activeIcon);
        }
    }

    /// <summary>
    /// Lets you add a weapontype to a key queue.
    /// </summary>
    /// <param name="weaponKeyQueue">The queue the weapon type should be added to.</param>
    /// <param name="type">The actual weapontype.</param>
    public void AddWeaponToKey(string weaponKeyQueue, ActiveWeaponType type)
    {
        if (weaponKeysDictionary.ContainsKey(weaponKeyQueue))
            weaponKeysDictionary[weaponKeyQueue].Enqueue(type);
    }

    /// <summary>
    /// Removes a weapontype from a key queue. In case the player should no longer see it or so.
    /// </summary>
    /// <param name="weaponKeyQueue">The queue to remove a type from.</param>
    /// <param name="typeToRemove">The type to remove.</param>
    public void RemoveWeaponFromKey(string weaponKeyQueue, ActiveWeaponType typeToRemove)
    {
        if (weaponKeysDictionary.ContainsKey(weaponKeyQueue))
        {
            if(weaponKeysDictionary[weaponKeyQueue].Count != 0)
            {
                ActiveWeaponType tmp = ActiveWeaponType.None;
                for (int i = 0; i < weaponKeysDictionary[weaponKeyQueue].Count; i++)
                {
                    tmp = weaponKeysDictionary[weaponKeyQueue].Peek();
                    if (tmp == typeToRemove)
                        weaponKeysDictionary[weaponKeyQueue].Dequeue();
                    else
                    {
                        weaponKeysDictionary[weaponKeyQueue].Enqueue(tmp);
                        weaponKeysDictionary[weaponKeyQueue].Dequeue();
                    }
                }
            }               
        }
    }

    /// <summary>
    /// A method used to pass the icon of the active weapon.
    /// </summary>
    /// <param name="iconName">The name of the activeweapon we need the icon from.</param>
    /// <returns>The icon of the weapon we asked for.</returns>
    private Sprite GetIcon(string iconName)
    {
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            if (weaponIcons[i].name == iconName)
                return weaponIcons[i].image;
        }
        return null;
    }
}
