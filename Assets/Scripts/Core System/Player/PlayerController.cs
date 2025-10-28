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

    }
}
