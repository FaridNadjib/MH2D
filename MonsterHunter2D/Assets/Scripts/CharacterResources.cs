using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterResources : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaRecoveryRate;
    [SerializeField] private float staminaRecoverDelay;
    private float staminaTimer = 0f;
    private bool canRecoverStamina = true;
    public bool HasStamina => stamina > 0;

    // An event that will get triggered once the unit died.
    public delegate void IsAlive ();
    public event IsAlive OnUnitDied;

    public delegate void HpChanged(float hpFillAmount);
    public event HpChanged OnHpChanged;
    public delegate void StaminaChanged(float staminaFillAmount);
    public event StaminaChanged OnStaminaChanged;

    private void Awake() 
    {
        health = maxHealth;
        stamina = maxStamina;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if(stamina < maxStamina && canRecoverStamina)
        {
            AddStamina(staminaRecoveryRate * Time.deltaTime);
        }

        if (!canRecoverStamina)
        {
            if(staminaTimer < staminaRecoverDelay)
            {
                staminaTimer += Time.deltaTime;
            }
            else
            {
                canRecoverStamina = true;
                staminaTimer = 0f;
            }
        }
    }

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

    public void AddHealth(float amount)
    {
        if (health + amount > maxHealth)
            health = maxHealth;
        else
            health += amount;

        OnHpChanged?.Invoke(GetCurrentHealthFraction());
    }

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

    public void AddStamina(float amount)
    {
        if (stamina + amount > maxStamina)
            stamina = maxStamina;
        else
            stamina += amount;

        OnStaminaChanged?.Invoke(GetCurrentStaminaFraction());
    }

    public void RestoreValues()
    {
        health = maxHealth;
        stamina = maxStamina;

        OnHpChanged?.Invoke(GetCurrentHealthFraction());
        OnStaminaChanged?.Invoke(GetCurrentStaminaFraction());
    }

    public float GetCurrentHealth()
    {
        return health;
    }

    public float GetCurrentHealthPercentage()
    {
        return health / maxHealth * 100;
    }

    public float GetCurrentHealthFraction()
    {
        return health / maxHealth;
    }

    public float GetCurrentStamina()
    {
        return stamina;
    }

    public float GetCurrentStaminaPercentage()
    {
        return stamina / maxStamina * 100;
    }

    public float GetCurrentStaminaFraction()
    {
        return stamina / maxStamina;
    }

}
