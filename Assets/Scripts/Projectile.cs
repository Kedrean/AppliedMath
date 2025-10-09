using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    public float hitDistance = 0.4f;
    public int damage = 1;

    private Vector2 moveDir;
    private EnemyController target;
    private bool hasHit = false;

    public void Launch(Vector2 dir, EnemyController targetEnemy)
    {
        moveDir = dir.normalized;
        target = targetEnemy;
    }

    void Update()
    {
        if (hasHit) return;

        transform.position += (Vector3)(moveDir * speed * Time.deltaTime);

        // Keep rotation facing direction
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (target != null)
        {
            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist <= hitDistance)
            {
                target.TakeDamage(damage);
                hasHit = true;
                Destroy(gameObject);
            }
        }

        // Destroy if far off screen
        if (Mathf.Abs(transform.position.x) > 15f || Mathf.Abs(transform.position.y) > 10f)
        {
            Destroy(gameObject);
        }
    }
}
