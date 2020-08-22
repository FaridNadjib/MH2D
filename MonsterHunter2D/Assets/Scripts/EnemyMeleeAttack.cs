using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private Enemy enemy;
    [SerializeField] private float recoilStrength;

    private void Awake() 
    {
        enemy = GetComponentInParent<Enemy>();    
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (other.GetComponent<PlayerController>().Invisible)
                return;

            if (enemy.CanHit)
            {
                enemy.CanHit = false;
                Vector3 direction = other.transform.position - transform.position;
                if (enemy is EnemyFish)
                    other.GetComponent<PlayerController>().ApplyRecoil(direction, recoilStrength, null, false);
                else
                    other.GetComponent<PlayerController>().ApplyRecoil(direction, recoilStrength, null, true);

                other.GetComponent<CharacterResources>().ReduceHealth(damage);
                enemy.HasHitPlayer(other);
            }
        }
    }
}