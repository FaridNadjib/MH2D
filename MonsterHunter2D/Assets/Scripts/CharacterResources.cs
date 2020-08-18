using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles our units stats like health and stamina and supplies some events for other classes like UIManager to listen to if those stats changed.
/// From Joachim and Farid.
/// </summary>
public class CharacterResources : MonoBehaviour
{
    #region Fields
    [Header("The units values:")]
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaRecoveryRate;
    [SerializeField] private float staminaRecoverDelay;
    private float staminaTimer = 0f;
    private bool canRecoverStamina = true;
    public bool HasStamina => stamina > 0;
    #endregion

    #region Events
    // An event that will get triggered once the unit died.
    public delegate void IsAlive ();
    public event IsAlive OnUnitDied;

    // Events that get triggered, when the units stats changed.
    public delegate void HpChanged(float hpFillAmount);
    public event HpChanged OnHpChanged;
    public delegate void StaminaChanged(float staminaFillAmount);
    public event StaminaChanged OnStaminaChanged;
    #endregion

    private void Awake() 
    {
        // Set them all to max values.
        health = maxHealth;
        stamina = maxStamina;
    }

    private void Update()
    {
        // This is only used for our player so far, update the staminacalculations.
        if(stamina < maxStamina && canRecoverStamina)
            AddStamina(staminaRecoveryRate * Time.deltaTime);

        if (!canRecoverStamina)
        {
            if(staminaTimer < staminaRecoverDelay)
                staminaTimer += Time.deltaTime;
            else
            {
                canRecoverStamina = true;
                staminaTimer = 0f;
            }
        }
    }

    /// <summary>
    /// Substracts health from the unit carrying this script. Once done an StatsChanged event gets triggered.
    /// </summary>
    /// <param name="amount">The amount to change the stat.</param>
    public void ReduceHealth(float amount)
    {
        if (health - amount < 0f)
            health = 0f;
        else
            health -= amount;

        if(health <= 0f)
            OnUnitDied?.Invoke();

        OnHpChanged?.Invoke(GetCurrentHealthFraction());
    }

    /// <summary>
    /// Adds health to the unit carrying this script. Once done an StatsChanged event gets triggered.
    /// </summary>
    /// <param name="amount">The amount to change the stat.</param>
    public void AddHealth(float amount)
    {
        if (health + amount > maxHealth)
            health = maxHealth;
        else
            health += amount;

        OnHpChanged?.Invoke(GetCurrentHealthFraction());
    }

    /// <summary>
    /// Substracts stamina from the unit carrying this script. Once done an StatsChanged event gets triggered.
    /// </summary>
    /// <param name="amount">The amount to change the stat.</param>
    public void ReduceStamina(float amount)
    {
        if (stamina - amount < 0f)
            stamina = 0f;
        else
            stamina -= amount;

        canRecoverStamina = false;
        staminaTimer = 0f;

        OnStaminaChanged?.Invoke(GetCurrentStaminaFraction());
    }

    /// <summary>
    /// Adds stamina to the unit carrying this script. Once done an StatsChanged event gets triggered.
    /// </summary>
    /// <param name="amount">The amount to change the stat.</param>
    public void AddStamina(float amount)
    {
        if (stamina + amount > maxStamina)
            stamina = maxStamina;
        else
            stamina += amount;

        OnStaminaChanged?.Invoke(GetCurrentStaminaFraction());
    }

    /// <summary>
    /// Set the stat to amount. Once done an StatsChanged event gets triggered.  
    /// </summary>
    /// <param name="amount">The amount to set the stat.</param>
    public void SetHealth(float amount)
    {
        maxHealth = amount;
        health = maxHealth;
        OnHpChanged?.Invoke(GetCurrentHealthFraction());
    }

    /// <summary>
    /// Set the stat to amount. Once done an StatsChanged event gets triggered.  
    /// </summary>
    /// <param name="amount">The amount to set the stat.</param>
    public void SetStamina(float amount)
    {
        maxStamina = amount;
        stamina = maxStamina;
        OnStaminaChanged?.Invoke(GetCurrentStaminaFraction());
    }

    /// <summary>
    /// Returns the current health.
    /// </summary>
    /// <returns>Current Health</returns>
    public float GetCurrentHealth()
    {
        return health;
    }

    /// <summary>
    /// Returns the current stamina.
    /// </summary>
    /// <returns>Current Stamina</returns>
    public float GetCurrentStamina()
    {
        return stamina;
    }

    /// <summary>
    /// Get the percentage of health from 0 to 1.
    /// </summary>
    /// <returns>Health percentage.</returns>
    public float GetCurrentHealthFraction()
    {
        return health / maxHealth;
    }

    /// <summary>
    /// Get the percentage of stamina from 0 to 1.
    /// </summary>
    /// <returns>Stamina percentage.</returns>
    public float GetCurrentStaminaFraction()
    {
        return stamina / maxStamina;
    }

}
