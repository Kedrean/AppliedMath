using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy")]
    public Vector3 size = new Vector3(1, 2, 1);
    public float speed = 2f;
    public float patrolRange = 4f;

    private int colliderID = -1;
    private Vector3 origin;
    private Vector3 dir = Vector3.left;

    void Start()
    {
        origin = transform.position;
        colliderID = CollisionManager.Instance.RegisterCollider(transform.position, size, false);
        CollisionManager.Instance.SetOwner(colliderID, gameObject);
        Matrix4x4 m = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
        CollisionManager.Instance.UpdateMatrix(colliderID, m);
    }

    void Update()
    {
        Vector3 pos = transform.position;
        Vector3 target = pos + dir * speed * Time.deltaTime;

        if (Mathf.Abs(target.x - origin.x) > patrolRange)
        {
            dir = -dir;
        }

        if (!CollisionManager.Instance.CheckCollision(colliderID, new Vector3(target.x, pos.y, pos.z), out List<int> colliding))
        {
            pos.x = target.x;
            transform.position = pos;

            // Update collider matrix and size every frame
            Matrix4x4 m = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
            CollisionManager.Instance.UpdateMatrix(colliderID, m);
            CollisionManager.Instance.UpdateCollider(colliderID, pos, size); // update size too
        }
        else
        {
            bool collidedWithPlayer = false;
            foreach (int id in colliding)
            {
                var owner = CollisionManager.Instance.GetOwner(id);
                if (owner != null && owner.GetComponent<PlayerController>() != null)
                {
                    collidedWithPlayer = true;
                    break;
                }
            }
            if (!collidedWithPlayer)
                dir = -dir;
        }
    }

    public void Die()
    {
        CollisionManager.Instance.RemoveCollider(colliderID);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (colliderID != -2)
            CollisionManager.Instance.RemoveCollider(colliderID);
    }
}
