using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float groundSpeed = 6f;
    public float jumpImpulse = 12f;
    public float gravity = 30f;
    public float gravityWhenFalling = 8f;
    [Tooltip("Horizontal speed is reduced by this factor while in air")]
    public float airSpeedMultiplier = 0.5f;

    [Header("Collision")]
    public Vector3 size = new Vector3(1, 2, 1);
    public Vector3 constantZPosition = Vector3.zero;

    [Header("Stats")]
    public int maxHP = 5;
    public int lives = 3;

    [Header("Invincibility")]
    public float invincibilityDuration = 3f;

    [Header("Prefabs & refs")]
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;
    public PlayerCameraFollow cameraFollow;
    public EnhancedMeshGenerator meshGenerator;

    // internal state
    private int colliderID = -1;
    private Vector3 velocity = Vector3.zero;
    private bool isGrounded = false;
    private bool isInvincible = false;
    private float invTimer = 0f;
    private int currentHP;
    private float inputHorizontal = 0f;

    void Start()
    {
        currentHP = maxHP;

        Vector3 startPos = transform.position;
        colliderID = CollisionManager.Instance.RegisterCollider(startPos, size, false);
        CollisionManager.Instance.SetOwner(colliderID, gameObject);
        Matrix4x4 m = Matrix4x4.TRS(startPos, Quaternion.identity, Vector3.one);
        CollisionManager.Instance.UpdateMatrix(colliderID, m);

        if (cameraFollow != null) cameraFollow.SetPlayerPosition(transform.position);

        Debug.Log($"Player colliderID: {colliderID} registered at position {startPos}");
    }

    void Update()
    {
        HandleInput();
        HandleInvincibility(Time.deltaTime);
        UpdateMovement(Time.deltaTime);
        UpdateCamera();
        UpdateUI();
    }

    void HandleInput()
    {
        inputHorizontal = 0f;
        if (Input.GetKey(KeyCode.A)) inputHorizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) inputHorizontal += 1f;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryJump();
        }

        if (Input.GetKeyDown(KeyCode.K) && fireballPrefab != null)
        {
            SpawnFireball();
        }
    }

    void TryJump()
    {
        if (isGrounded)
        {
            velocity.y = jumpImpulse;
            isGrounded = false;
        }
    }

    void HandleInvincibility(float dt)
    {
        if (isInvincible)
        {
            invTimer -= dt;
            if (invTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }

    void UpdateMovement(float dt)
    {
        if (colliderID == -1) return;

        float speed = groundSpeed;
        if (!isGrounded) speed *= airSpeedMultiplier;

        Vector3 pos = CollisionManager.Instance.GetMatrix(colliderID).GetPosition();

        // Horizontal movement and collision check
        float newX = pos.x + inputHorizontal * speed * dt;
        if (!CheckCollisionAt(colliderID, new Vector3(newX, pos.y, pos.z)))
        {
            pos.x = newX;
        }

        // Vertical movement and gravity
        if (velocity.y > 0f)
        {
            velocity.y -= gravity * dt * 0.5f;
        }
        else
        {
            velocity.y -= gravityWhenFalling * dt;
        }

        Vector3 newPos = pos;
        newPos.y += velocity.y * dt;

        if (CheckCollisionAt(colliderID, new Vector3(pos.x, newPos.y, pos.z)))
        {
            if (velocity.y < 0f)
            {
                isGrounded = true;
            }
            velocity.y = 0f;
        }
        else
        {
            pos.y = newPos.y;
            isGrounded = false;
        }

        if (meshGenerator != null)
        {
            float groundY = meshGenerator.groundY + size.y * 0.5f;
            if (pos.y < groundY - 50f)
            {
                DieAndRespawn();
                return;
            }
        }

        // Apply final position
        transform.position = pos;

        // Update collider matrix and size to match new position
        Matrix4x4 m = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
        CollisionManager.Instance.UpdateMatrix(colliderID, m);
        CollisionManager.Instance.UpdateCollider(colliderID, pos, size);

        bool collided = CollisionManager.Instance.CheckCollision(colliderID, pos, out List<int> colliding);
        Debug.Log($"Collision check for player: collided={collided}, colliding count={colliding?.Count ?? 0}");


        // **Check collisions after movement update and collider update**
        if (collided && colliding != null && colliding.Count > 0)
        {
            Debug.Log($"Player colliding with {colliding.Count} objects");
            foreach (int otherID in colliding)
            {
                GameObject owner = CollisionManager.Instance.GetOwner(otherID);
                if (owner == null)
                {
                    Debug.LogWarning($"Collider ID {otherID} has no owner!");
                    continue;
                }

                Debug.Log($"Player collided with {owner.name}");

                var power = owner.GetComponent<PowerupPickup>();
                if (power != null)
                {
                    power.OnPickup(this);
                    CollisionManager.Instance.RemoveCollider(otherID);
                    Destroy(owner);
                    continue;
                }

                var enemy = owner.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Debug.Log("Collision with Enemy detected.");
                    OnEnemyCollision(enemy);
                    continue;
                }

                var instakill = owner.GetComponent<InstakillObstacle>();
                if (instakill != null)
                {
                    Debug.Log("Collision with InstakillObstacle detected.");
                    TakeDamage(currentHP); // instant death
                    continue;
                }

                var goal = owner.GetComponent<Goal>();
                if (goal != null)
                {
                    GameManager.Instance.LevelComplete();
                }
            }
        }
    }

    void OnEnemyCollision(Enemy enemy)
    {
        if (isInvincible)
        {
            enemy.Die();
            return;
        }
        TakeDamage(1);
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible)
        {
            Debug.Log("Player is invincible, no damage taken.");
            return;
        }

        currentHP -= amount;
        Debug.Log($"Player took {amount} damage, HP now {currentHP}");

        if (currentHP <= 0)
        {
            Debug.Log("Player died.");
            DieAndRespawn();
        }
    }

    void DieAndRespawn()
    {
        lives--;
        if (lives <= 0)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Vector3 spawn = new Vector3(0, 10, 0);
            transform.position = spawn;
            velocity = Vector3.zero;
            currentHP = maxHP;
            Matrix4x4 m = Matrix4x4.TRS(spawn, Quaternion.identity, Vector3.one);
            CollisionManager.Instance.UpdateMatrix(colliderID, m);
            CollisionManager.Instance.UpdateCollider(colliderID, spawn, size);
            if (cameraFollow != null) cameraFollow.SetPlayerPosition(spawn);
        }
    }

    void SpawnFireball()
    {
        if (fireballPrefab == null) return;
        Vector3 spawnPos = fireballSpawnPoint != null ? fireballSpawnPoint.position : transform.position + Vector3.right * 1.2f;

        // Spawn fireball with rotation so it is horizontal (Z rotation -90)
        Quaternion spawnRotation = Quaternion.Euler(0, 0, -90);

        GameObject fb = Instantiate(fireballPrefab, spawnPos, spawnRotation);
        var fscript = fb.GetComponent<Fireball>();
        if (fscript != null) fscript.Initialize(Vector3.right);
    }

    public void GrantExtraLife(int amount = 1)
    {
        lives += amount;
    }

    public void GrantInvincibility(float duration)
    {
        isInvincible = true;
        invTimer = Mathf.Max(invTimer, duration);
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    bool CheckCollisionAt(int id, Vector3 pos)
    {
        return CollisionManager.Instance.CheckCollision(id, pos, out _);
    }

    void UpdateCamera()
    {
        if (cameraFollow != null) cameraFollow.SetPlayerPosition(transform.position);
    }

    void UpdateUI()
    {
        UIManager.Instance?.UpdateHP(currentHP, maxHP);
        UIManager.Instance?.UpdateLives(lives);
    }

    public int GetLives() => lives;
    public int GetHP() => currentHP;
    public void AddHP(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }

    // Optional for InstakillObstacle compatibility
    public void Die()
    {
        DieAndRespawn();
    }
}
