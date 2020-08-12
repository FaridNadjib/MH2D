using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileExplodingTail : EnemyProjectile
{
    [SerializeField] private Vector2Int minMaxNumberOfSpikes;
    [SerializeField] private float secondsToExplosion;

    private bool exploded;

    protected new const string pool = "ankyloExplodingTailPool";
    private const string spikePool = "ankyloSpikePool";

    private bool timerStarted;

    private void OnEnable() 
    {
        timerStarted = true;
    }

    protected override void Update() 
    {
        Timer();
        RotateTowardsDirection();
        FadeOut();
    }

    private void Timer()
    {
        if (!timerStarted)
            return;

        secondsToExplosion -= Time.deltaTime;

        if (secondsToExplosion <= 0f && !exploded)
            Explode();  
    }

    private void Explode()
    {
        GetComponent<AudioSource>().Play();

        exploded = true;

        int spikes = UnityEngine.Random.Range(minMaxNumberOfSpikes.x, minMaxNumberOfSpikes.y);

        float angleStep = 360f / spikes;
        float angle = UnityEngine.Random.Range(0, 359);

        for (int i = 0; i < spikes; i++)
        {
            GameObject tempTail = ObjectPoolsController.instance.GetFromPool(spikePool);
            Vector2 startPos = new Vector2(transform.position.x, transform.position.y);
            tempTail.transform.position = startPos;

            float directionX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float directionY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);

            Vector2 projectileVector = new Vector2(directionX, directionY);
            Vector2 projectileForce = (projectileVector - startPos).normalized * tempTail.GetComponent<Projectile>().projectileSpeed;

            tempTail.SetActive(true);
            tempTail.GetComponent<Projectile>().ShootProjectile(projectileForce.normalized);

            angle += angleStep + UnityEngine.Random.Range(0, 44);
        } 
    }
}