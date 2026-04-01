using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [SerializeField] float damage = 15f;

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // destroy on hitting anything that isn't the boss or another projectile
        if (other.GetComponent<SolomonBoss>() != null) return;
        if (other.GetComponent<BossProjectile>() != null) return;

        Destroy(gameObject);
    }
}