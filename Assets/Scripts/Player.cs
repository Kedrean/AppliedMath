using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    public GameObject rocketPrefab;
    public float fireInterval = 2f;
    private float fireTimer;
    public int rocketCount = 4;
    public int maxRockets = 8;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fireTimer = fireInterval;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireRockets();
            fireTimer = fireInterval;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void FireRockets()
    {
        float angleStep = 360f / rocketCount;
        float angle = 0f;

        for (int i = 0; i < rocketCount; i++)
        {
            float rocketDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float rocketDirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector2 rocketDir = new Vector2(rocketDirX, rocketDirY).normalized;

            GameObject rocket = Instantiate(rocketPrefab, transform.position, Quaternion.identity);
            rocket.GetComponent<Rocket>().SetDirection(rocketDir);

            angle += angleStep;
        }
    }

    public void IncreaseRocketCount()
    {
        if (rocketCount < maxRockets)
        {
            rocketCount++;
        }
    }

    public void DecreaseRocketCount()
    {
        if (rocketCount > 1)
        {
            rocketCount--;
        }
    }
}
