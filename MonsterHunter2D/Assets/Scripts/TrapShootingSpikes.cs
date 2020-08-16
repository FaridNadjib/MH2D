using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapShootingSpikes : MonoBehaviour
{   
    [Tooltip("Should this trap be triggered?")]
    [SerializeField] private bool triggeredTrap;
    private bool triggered = false;
    private bool shotOnce = false;
    [SerializeField] private Vector2 minMaxShootingInterval;
    [SerializeField] private Transform spawnPos;
    private float currentShootingInterval;
    private float currentTime;

    private const string spikePool = "shootingSpikePool";

    private void Awake() 
    {
        currentShootingInterval = GetNewInterval();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null && triggeredTrap)
            triggered = true;
    }

    private void Update() 
    {
        if (triggeredTrap)
        {
            if (triggered && !shotOnce)
            {
                shotOnce = true;
                Shoot();
            }
            else if (triggered && Waited())
                Shoot();
        }
        else if (Waited())
            Shoot();
    }

    private void Shoot()
    {
        currentTime = 0f;

        GameObject spike = ObjectPoolsController.instance.GetFromPool(spikePool);

        spike.transform.position = spawnPos.position;

        Vector2 direction = transform.up;
        spike.SetActive(true);
        spike.GetComponent<Projectile>().ShootProjectile(direction.normalized, false);

        currentShootingInterval = GetNewInterval();
    }

    private bool Waited()
    {
        currentTime += Time.deltaTime;

        if (currentTime < currentShootingInterval)
            return false;
        else
            return true;
    }

    private float GetNewInterval()
    {
        return UnityEngine.Random.Range(minMaxShootingInterval.x, minMaxShootingInterval.y);
    }


}
