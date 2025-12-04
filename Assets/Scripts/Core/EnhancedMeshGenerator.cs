using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;

/// Only creates the ground mesh as a cube, starting at origin point,
/// and its collider.
public class EnhancedMeshGenerator : MonoBehaviour
{
    [Header("Mesh Settings")]
    public Material material;

    [Header("Ground Settings")]
    public float groundY = -20f;
    public float groundWidth = 200f;
    public float groundDepth = 200f;
    public float groundHeight = 1f; // Thickness of ground cube
    public float constantZPosition = 0f;

    private Mesh groundMesh;
    private Matrix4x4 groundMatrix;

    void Start()
    {
        CreateGroundMesh();
        CreateGroundCollider();
    }

    void CreateGroundMesh()
    {
        groundMesh = new Mesh();

        // Vertices for cube starting at origin (0,0,0) to (width, height, depth)
        Vector3[] vertices = new Vector3[8]
        {
            new Vector3(0, 0, 0),                          // bottom 0
            new Vector3(groundWidth, 0, 0),                // bottom 1
            new Vector3(groundWidth, 0, groundDepth),      // bottom 2
            new Vector3(0, 0, groundDepth),                 // bottom 3

            new Vector3(0, groundHeight, 0),                // top 4
            new Vector3(groundWidth, groundHeight, 0),     // top 5
            new Vector3(groundWidth, groundHeight, groundDepth), // top 6
            new Vector3(0, groundHeight, groundDepth)      // top 7
        };

        // Triangles for cube (12 triangles, 36 indices)
        int[] triangles = new int[]
        {
            // Bottom
            0, 2, 1,
            0, 3, 2,

            // Top
            4, 5, 6,
            4, 6, 7,

            // Front
            0, 1, 5,
            0, 5, 4,

            // Back
            3, 7, 6,
            3, 6, 2,

            // Left
            0, 4, 7,
            0, 7, 3,

            // Right
            1, 2, 6,
            1, 6, 5
        };

        Vector2[] uvs = new Vector2[8]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),

            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        groundMesh.vertices = vertices;
        groundMesh.triangles = triangles;
        groundMesh.uv = uvs;

        groundMesh.RecalculateNormals();

        // Position the ground at (0, groundY, constantZPosition)
        groundMatrix = Matrix4x4.TRS(
            new Vector3(0, groundY, constantZPosition),
            Quaternion.identity,
            Vector3.one
        );
    }

    void CreateGroundCollider()
    {
        Vector3 pos = new Vector3(groundWidth / 2f, groundY + groundHeight / 2f, constantZPosition + groundDepth / 2f);
        Vector3 size = new Vector3(groundWidth, groundHeight, groundDepth);

        int groundID = CollisionManager.Instance.RegisterCollider(pos, size, false);
        CollisionManager.Instance.UpdateMatrix(groundID, Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one));
    }

    void Update()
    {
        if (groundMesh != null)
        {
            Graphics.DrawMesh(groundMesh, groundMatrix, material, 0);
        }
    }
}
