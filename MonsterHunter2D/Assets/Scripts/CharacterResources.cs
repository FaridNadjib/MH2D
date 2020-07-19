using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterResources : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;

    

    private void Awake() 
    {
        health = maxHealth;
    }

    public void ReduceHealth(int amount)
    {
        if (health - amount < 0)
            health = 0;
        else
            health -= amount;
    }

    public void AddHealth(int amount)
    {
        if (health + amount > maxHealth)
            health = maxHealth;
        else
            health += amount;
    }

    public int GetCurrentHealth()
    {
        return health;
    }

    public int GetCurrentHealthPercentage()
    {
        return health / maxHealth * 100;
    }

    public int GetCurrentHealthFraction()
    {
        return health / maxHealth;
    }

}
