using UnityEngine;

public class PlayerController : MonoBehaviour, IMovable
{
    [Header("Movement")]
    public float laneOffset = 2.5f;
    public float laneSwitchSpeed = 10f;
    public float jumpVelocity = 8f;
    public float gravity = -20f;
    public float groundY = 0f;

    [Header("HP")]
    public int maxHP = 100;
    public float hpRegenPerSec = 1f;

    [Header("Animation")]
    public Animator animator;

    private int _lane = 1;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        UpdateLanePosition();

        UIManager.Instance.UpdateHP(_health.currentHP, _health.maxHP);
    }

    // Update is called once per frame
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
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            ChangeLane(1);
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Approximately(transform.position.y, groundY))
        {
            _yVelocity = jumpVelocity;
            animator?.SetTrigger("Jump");
        }
    }

    private void ChangeLane(int dir)
    {
        _lane = Mathf.Clamp(_lane + dir, 0, 2);
        UpdateLanePosition();
        animator?.SetTrigger("SwitchLane");
    }

    private void UpdateLanePosition()
    {
        float x = (_lane - 1) * laneOffset;
        _targetPos = new Vector3(x, transform.position.y, transform.position.z);
    }

    private void MoveLane(float deltaTime)
    {
        Vector3 pos = transform.position;
        float step = laneSwitchSpeed * deltaTime;
        float dx = _targetPos.x - pos.x;
        float moveX = Mathf.Sign(dx) * Mathf.Min(Mathf.Abs(dx), step);
        pos.x += moveX;
        transform.position = new Vector3(pos.x, pos.y, pos.z);
    }

    private void ApplyJump(float deltaTime)
    {
        _yVelocity += gravity * deltaTime;
        float y = transform.position.y + _yVelocity * deltaTime;
        if (y <= groundY)
        {
            y = groundY;
            _yVelocity = 0f;
        }
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public void TakeDamage(int dmg)
    {
        _health.TakeDamage(dmg);
        animator?.SetTrigger("Hit");
        if (_health.currentHP == 0) Die();
    }

    private void Die()
    {
        animator?.SetTrigger("Die");
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
        animator.SetBool("Running", Mathf.Approximately(transform.position.y, groundY));
        animator.SetBool("MovingHoriz", Mathf.Abs(transform.position.x - _targetPos.x) > 0.01f);
    }
}
