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
        }
    }
}