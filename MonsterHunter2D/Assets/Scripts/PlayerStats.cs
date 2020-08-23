using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object that lets us save all the player related values.
/// </summary>
[CreateAssetMenu(menuName = "Player Stats")]
public class PlayerStats : ScriptableObject
{
    #region Default Values to restore if needed
    [SerializeField] int defHeartNumber;
    [SerializeField] float defHealth;
    [SerializeField] float defStamina;
    [SerializeField] float defInvisibility;
    [SerializeField] int defArrow;
    [SerializeField] int defSpears;
    [SerializeField] int defPlatformSpears;
    [SerializeField] int defBombNormal;
    [SerializeField] int defStickyBomb;
    [SerializeField] int defMegaBomb;
    #endregion

    #region Properties
    public int CurrentCrystals { get ; set ; }
    public int NumberOfHearts { get; set; }
    public int CurrentNumberOfHearts { get; set; }
    public float MaxHealth { get; set; }
    public float MaxStamina { get; set; }
    public float MaxInvisibilityTime { get; set; }
    public int MaxArrows { get; set; }
    public int MaxSpears { get; set; }
    public int MaxPlatformspears { get; set; }
    public int MaxBombNormal { get; set; }
    public int MaxStickyBomb { get; set; }
    public int MaxMegaBomb { get; set; }

    public float SpawnPosX { get; set; } = 666;
    public float SpawnPosY { get; set; } = 666;

    // Two properties to update to the correct munition type.
    public ActiveWeaponType WeaponType { get; set; }
    public int Amount { get; set; }
    #endregion

    // Events to let subscribers know that something changed, for UI for example.
    public event StatsChanged OnStatsChanged;
    public delegate void StatsChanged();

    /// <summary>
    /// Call this method anywhere when you want to restore the values of the scriptable object to default values you set in the inspector.
    /// </summary>
    public void ResetPlayerStats()
    {
        CurrentCrystals = 0;
        NumberOfHearts = defHeartNumber;
        CurrentNumberOfHearts = NumberOfHearts;
        MaxHealth = defHealth;
        MaxStamina = defStamina;
        MaxInvisibilityTime = defInvisibility;
        MaxArrows = defArrow;
        MaxSpears = defSpears;
        MaxPlatformspears = defPlatformSpears;
        MaxBombNormal = defBombNormal;
        MaxStickyBomb = defStickyBomb;
        MaxMegaBomb = defMegaBomb;
    }

    /// <summary>
    /// Call this method if a value has been changed, the triggered event will notifiy all listeners.
    /// </summary>
    public void ValuesChanged()
    {
        OnStatsChanged?.Invoke();
    }

    /// <summary>
    /// Deletes the current Spawnposition by setting it on a default value.
    /// </summary>
    public void DeleteSpawnPos()
    {
        SpawnPosX = 666;
    }
}
