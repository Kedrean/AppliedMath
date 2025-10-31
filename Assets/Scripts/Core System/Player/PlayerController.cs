using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour, IMovable
{
    [Header("HP")]
    public int maxHP;
    public float hpRegenPerSec;

    [Header("Movement")]
    public float laneSwitchSpeed;
    public float jumpHeight;
    public float gravity;
    public float groundY;
    public float spawnZ;
    public float leftLaneX;
    public float middleLaneX;
    public float rightLaneX;
    public float laneOffset => Mathf.Abs(rightLaneX - middleLaneX);

    [Header("Animation")]
    public Animator animator;

    public bool IsEvading => transform.position.y > groundY + 0.1f;

    private int _lane = 1; // 0 = left, 1 = middle, 2 = right
    private float _yVelocity;
    private Vector3 _targetPos;

    private IHealth _health;
    private ICollisionChecker _collisionChecker;

    public int Lane => _lane;
    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        _health = new PlayerHealth(maxHP, hpRegenPerSec);
        _collisionChecker = new PlayerCollisionChecker(transform, 1.5f, () => _lane);

        ((PlayerHealth)_health).HealthChanged += OnHealthChanged;
    }

    private void Start()
    {
        // Spawn player in middle lane
        _lane = 1;
        _targetPos = new Vector3(middleLaneX, groundY, spawnZ);
        transform.position = _targetPos;
        UpdateLanePosition();

        UIManager.Instance.UpdateHP(_health.currentHP, _health.maxHP);
    }

    private void Update()
    {
        HandleInput();
        Move(Time.deltaTime);

        _health.Regenerate(Time.deltaTime);
        UpdateAnimator();
    }

    public void Move(float deltaTime)
    {
        MoveLane(deltaTime);
        ApplyJump(deltaTime);
    }

    private void HandleInput()
    {
        // Move Left
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
            animator?.ResetTrigger("MoveRight");
            animator?.SetTrigger("MoveLeft");
        }

        // Move Right
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
            animator?.ResetTrigger("MoveLeft");
            animator?.SetTrigger("MoveRight");
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Approximately(transform.position.y, groundY))
        {
            _yVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
            animator?.SetTrigger("Jump");
        }
    }

    private void ChangeLane(int dir)
    {
        _lane = Mathf.Clamp(_lane + dir, 0, 2);
        UpdateLanePosition();
    }

    private void UpdateLanePosition()
    {
        float x = _lane switch
        {
            0 => leftLaneX,
            1 => middleLaneX,
            2 => rightLaneX,
            _ => middleLaneX
        };
        _targetPos = new Vector3(x, groundY, spawnZ);
    }

    private void MoveLane(float deltaTime)
    {
        Vector3 pos = transform.position;
        float step = laneSwitchSpeed * deltaTime;
        float dx = _targetPos.x - pos.x;
        float moveX = Mathf.Sign(dx) * Mathf.Min(Mathf.Abs(dx), step);
        pos.x += moveX;

        pos.z = spawnZ; // keep Z constant
        transform.position = pos;
    }

    private void ApplyJump(float deltaTime)
    {
        // Apply gravity
        _yVelocity -= gravity * deltaTime;

        // Update Y position
        float newY = transform.position.y + _yVelocity * deltaTime;

        // Clamp to ground
        if (newY <= groundY)
        {
            newY = groundY;
            _yVelocity = 0f;
        }

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void TakeDamage(int dmg)
    {
        _health.TakeDamage(dmg);
        if (_health.currentHP == 0)
            Die();
    }

    private void Die()
    {
        animator?.SetTrigger("Die");
        StartCoroutine(DieAfterAnimation());
    }

    private IEnumerator DieAfterAnimation()
    {
        // Wait for the animation to finish
        if (animator != null)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            float clipLength = state.length;
            yield return new WaitForSeconds(clipLength);
        }

        GameManager.Instance.OnPlayerDead();
    }

    private void OnHealthChanged(int hp, int max)
    {
        UIManager.Instance.UpdateHP(hp, max);
    }

    public bool IsHitBy(Vector3 obstaclePos, int lane)
    {
        return _collisionChecker.CheckCollision(obstaclePos, lane);
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        bool isGrounded = Mathf.Approximately(transform.position.y, groundY);
        bool isMovingHorizontally = Mathf.Abs(transform.position.x - _targetPos.x) > 0.01f;

        // Idle = grounded + not moving horizontally
        animator.SetBool("Idle", isGrounded && !isMovingHorizontally);
    }
}
