using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Fireball")]
    public float speed = 10f;
    public float lifetime = 5f;
    public Vector3 size = new Vector3(0.6f, 0.6f, 0.6f);

    private Vector3 direction = Vector3.right;
    private int colliderID = -1;
    private float lifeTimer;

    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        lifeTimer = lifetime;
        colliderID = CollisionManager.Instance.RegisterCollider(transform.position, size, false);
        CollisionManager.Instance.SetOwner(colliderID, gameObject);
        CollisionManager.Instance.UpdateMatrix(colliderID, Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one));
    }

    void Update()
    {
        float dt = Time.deltaTime;
        Vector3 pos = transform.position;
        Vector3 newPos = pos + direction * speed * dt;

        // Move
        transform.position = newPos;
        CollisionManager.Instance.UpdateMatrix(colliderID, Matrix4x4.TRS(newPos, Quaternion.identity, Vector3.one));
        CollisionManager.Instance.UpdateCollider(colliderID, newPos, size);

        if (CollisionManager.Instance.CheckCollision(colliderID, newPos, out List<int> colliding))
        {
            foreach (int id in colliding)
            {
                GameObject owner = CollisionManager.Instance.GetOwner(id);
                if (owner == null) continue;

                var enemy = owner.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Die();
                    Explode();
                    return;
                }

                // obstacles -> destroy both
                var instakill = owner.GetComponent<InstakillObstacle>();
                if (instakill != null)
                {
                    Destroy(owner);
                    Explode();
                    return;
                }
            }
        }

        lifeTimer -= dt;
        if (lifeTimer <= 0f) Explode();
    }

    void Explode()
    {
        CollisionManager.Instance.RemoveCollider(colliderID);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (colliderID != -1) CollisionManager.Instance.RemoveCollider(colliderID);
    }
}
