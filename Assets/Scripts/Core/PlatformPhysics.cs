using UnityEngine;

public class PlatformPhysics : MonoBehaviour
{
    private Vector3 halfExtents;

    private void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Bounds b = mf.sharedMesh.bounds;
        halfExtents = b.extents * transform.localScale.x;
    }

    public float TopY()
    {
        return transform.position.y + halfExtents.y;
    }

    public Bounds GetAABB()
    {
        return new Bounds(transform.position, halfExtents * 2f);
    }
}
