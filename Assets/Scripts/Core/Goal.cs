using UnityEngine;

public class Goal : MonoBehaviour
{
    public Vector3 size = new Vector3(1, 2, 1);
    private int colliderID = -1;

    void Start()
    {
        colliderID = CollisionManager.Instance.RegisterCollider(transform.position, size, false);
        CollisionManager.Instance.SetOwner(colliderID, gameObject);
        CollisionManager.Instance.UpdateMatrix(colliderID, Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one));
    }

    void OnDestroy()
    {
        if (colliderID != -1) CollisionManager.Instance.RemoveCollider(colliderID);
    }
}
