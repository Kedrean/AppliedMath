using UnityEngine;
using System.Linq;

public class Turret : MonoBehaviour
{
    [Header("References")]
    public Transform barrel;
    public EnemySpawner spawner;
    public GameObject projectilePrefab;

    [Header("Turret Settings")]
    public float rotationSpeed = 180f;
    public float fireRange = 6f;
    public float fireCooldown = 1.5f;
    public float muzzleOffset = 0.5f;

    private float cooldownTimer = 0f;

    void Update()
    {
        if (spawner == null) return;

        var enemies = spawner.GetActiveEnemies();
        if (enemies == null || enemies.Count == 0) return;

        // Find nearest alive enemy
        EnemyController target = enemies
            .Where(e => e != null && !e.IsDead)
            .OrderBy(e => Vector2.Distance(transform.position, e.transform.position))
            .FirstOrDefault();

        if (target == null) return;

        Vector2 dir = target.transform.position - transform.position;
        float dist = dir.magnitude;

        // Rotate entire turret
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newAngle);

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f && dist <= fireRange)
        {
            Fire(target);
            cooldownTimer = fireCooldown;
        }
    }

    void Fire(EnemyController target)
    {
        Vector3 muzzlePos = barrel.position + barrel.right * muzzleOffset;
        GameObject proj = Instantiate(projectilePrefab, muzzlePos, barrel.rotation);

        Projectile p = proj.GetComponent<Projectile>();
        Vector2 dir = barrel.right.normalized;
        p.Launch(dir, target);
    }
}
