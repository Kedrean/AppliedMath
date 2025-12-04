using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    public enum PowerType { Fireball }
    public PowerType powerType = PowerType.Fireball;

    [Header("Fireball settings")]
    public GameObject fireballPrefab;
    public float invincibilitySeconds = 4f;
    public int extraLives = 1;

    private int colliderID = -1;

    void Start()
    {
        // Register collider with CollisionManager - assume this object has simple bounds
        Vector3 pos = transform.position;
        Vector3 size = new Vector3(1f, 1f, 1f);
        colliderID = CollisionManager.Instance.RegisterCollider(pos, size, false);
        CollisionManager.Instance.SetOwner(colliderID, gameObject);
        CollisionManager.Instance.UpdateMatrix(colliderID, Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one));
    }

    public void OnPickup(PlayerController player)
    {
        if (powerType == PowerType.Fireball)
        {
            player.GrantExtraLife(extraLives);
            player.GrantInvincibility(invincibilitySeconds);

            // spawn a fireball traveling to the right from the pickup position
            if (fireballPrefab != null)
            {
                GameObject fb = Instantiate(fireballPrefab, transform.position + Vector3.right * 1.2f, Quaternion.identity);
                var fscript = fb.GetComponent<Fireball>();
                if (fscript != null) fscript.Initialize(Vector3.right);
            }
        }
    }

    void OnDestroy()
    {
        if (colliderID != -1) CollisionManager.Instance.RemoveCollider(colliderID);
    }
}
