using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Stats")]
public class PlayerStats : ScriptableObject
{
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
    

    public event StatsChanged OnStatsChanged;
    public delegate void StatsChanged();

    // Start is called before the first frame update
    void Start()
    {
        
    }


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

        Debug.Log("Resettet the playerstats.");
    }

    /// <summary>
    /// Call this method if a value has been changed, the triggered event will notifiy all listeners.
    /// </summary>
    public void ValuesChanged()
    {
        OnStatsChanged?.Invoke();
    }

    public void DeleteSpawnPos()
    {
        SpawnPosX = 666;
    }
}
