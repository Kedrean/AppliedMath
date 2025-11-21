using UnityEngine;

public class CubePhysicsController : MonoBehaviour
{
    public float gravity = -12f;
    public float jumpForce = 6f;
    public bool isGrounded = false;

    private float verticalVelocity = 0f;
    private MeshRenderer renderer;

    private Vector3 halfExtents;
    private Color originalColor;

    private void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        originalColor = renderer.material.color; // save original color

        MeshFilter mf = GetComponent<MeshFilter>();
        Bounds b = mf.sharedMesh.bounds;
        halfExtents = Vector3.Scale(b.extents, transform.localScale); // scale extents correctly
    }

    private void Update()
    {
        HandleGravity();
        HandleJump();
    }

    private void HandleGravity()
    {
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 pos = transform.position;
        pos.y += verticalVelocity * Time.deltaTime;

        transform.position = pos;

        CheckPlatformCollision();
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = jumpForce;
            isGrounded = false;
        }
    }

    private void CheckPlatformCollision()
    {
        bool collided = false;

        PlatformPhysics[] platforms = Object.FindObjectsByType<PlatformPhysics>(FindObjectsSortMode.None);

        foreach (var p in platforms)
        {
            if (AABBOverlap(this, p))
            {
                float top = p.TopY();

                // Snap cube to top of platform precisely (top + half cube height)
                Vector3 pos = transform.position;
                pos.y = top + halfExtents.y;
                transform.position = pos;

                verticalVelocity = 0f;
                isGrounded = true;

                collided = true;
                break; // only handle first collision
            }
        }

        // Change color based on collision state
        if (collided)
            renderer.material.color = Color.green;
        else
        {
            renderer.material.color = originalColor;
            isGrounded = false;
        }
    }

    private bool AABBOverlap(CubePhysicsController cube, PlatformPhysics platform)
    {
        Bounds cb = cube.GetAABB();
        Bounds pb = platform.GetAABB();

        return cb.min.x <= pb.max.x &&
               cb.max.x >= pb.min.x &&
               cb.min.y <= pb.max.y &&
               cb.max.y >= pb.min.y &&
               cb.min.z <= pb.max.z &&
               cb.max.z >= pb.min.z;
    }

    private Bounds GetAABB()
    {
        return new Bounds(transform.position, halfExtents * 2f);
    }
}
