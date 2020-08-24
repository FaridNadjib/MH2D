using UnityEngine;

/// <summary>
/// the ankylos exploding tail projectile.!-- Joachim
/// </summary>
public class EnemyProjectileExplodingTail : EnemyProjectile
{
    [SerializeField] private Vector2Int minMaxNumberOfSpikes;
    [SerializeField] private float secondsToExplosion;

    private float currentTime = 0f;

    private bool exploded;

    protected new const string pool = "ankyloExplodingTailPool";
    private const string spikePool = "ankyloSpikePool";

    private bool timerStarted;

    protected override void OnEnable() 
    {
        currentTime = 0f;
        timerStarted = true;
        exploded = false;

        Setup();
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

        currentTime += Time.deltaTime;

        if (currentTime >= secondsToExplosion && !exploded)
            Explode();  
    }

    /// <summary>
    /// lets the projectile explode into spikes in a circular facing direction with a random offset
    /// </summary>
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
            tempTail.GetComponent<Projectile>().ShootProjectile(projectileForce.normalized, false);

            angle += angleStep + UnityEngine.Random.Range(0, 44);
        } 
    }
}