using UnityEngine;
using System.Collections;

public class NinjaEnemy : Enemy
{
    [Header("Ninja Settings")]
    [SerializeField] private GameObject shurikenPrefab;
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float fireCooldown = 2f;
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstDelay = 0.15f;
    [SerializeField] private float shurikenSpeed = 8f;
    [SerializeField] private float shurikenDamage = 10f;

    private float fireTimer = 0f;
    private bool isFiring = false;

    protected override void Start()
    {
        base.Start();

        Player player = FindFirstObjectByType<Player>();
        if (player != null) target = player.transform;
    }

    protected override void HandleAI()
    {
        if (target == null || isFiring) return;

        // stand still
        rb.linearVelocity = Vector2.zero;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > attackRange)
        {
            fireTimer = 0f;
            return;
        }

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireCooldown)
        {
            fireTimer = 0f;
            StartCoroutine(BurstFire());
        }
    }

    private IEnumerator BurstFire()
    {
        isFiring = true;

        for (int i = 0; i < burstCount; i++)
        {
            FireShuriken();
            yield return new WaitForSeconds(burstDelay);
        }

        isFiring = false;
    }

    private void FireShuriken()
    {
        if (shurikenPrefab == null || target == null) return;

        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject obj = Instantiate(shurikenPrefab, transform.position, Quaternion.Euler(0, 0, angle));

        ShurikenProjectile proj = obj.GetComponent<ShurikenProjectile>();
        if (proj != null)
            proj.Initialize(dir, shurikenSpeed, shurikenDamage);
    }
}