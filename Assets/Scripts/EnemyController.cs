using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 0.15f;
    public float curveDepth = 3f;

    [Header("Health Settings")]
    public int maxHealth = 2;
    public Image healthBarFill;

    private int currentHealth;
    private float t = 0f;
    private Vector3[] p;   // array of control points
    public bool IsDead => currentHealth <= 0;

    void Start()
    {
        // We'll use 8 control points (2 cubic segments)
        p = new Vector3[8];

        // Left cubic segment
        p[0] = new Vector3(-10f, 0f, 0f);          // start
        p[1] = new Vector3(-6f, 0f, 0f);           // flat line
        p[2] = new Vector3(-2f, -curveDepth, 0f);  // descend
        p[3] = new Vector3(0f, -curveDepth, 0f);   // bottom mid

        // Right cubic segment
        p[4] = p[3];                               // start second segment at same point
        p[5] = new Vector3(2f, -curveDepth, 0f);   // ascend
        p[6] = new Vector3(6f, 0f, 0f);            // flatten out
        p[7] = new Vector3(10f, 0f, 0f);           // exit point

        currentHealth = maxHealth;
        transform.position = p[0];
    }

    void Update()
    {
        if (IsDead)
        {
            Destroy(gameObject);
            return;
        }

        t += Time.deltaTime * speed;

        if (t >= 1f)
        {
            Destroy(gameObject); // reached end
            return;
        }

        Vector3 pos;

        // Half of the timeline is left curve, half is right curve
        if (t < 0.5f)
        {
            float localT = t * 2f; // scale 0–0.5 → 0–1
            pos = CalculateCubicBezierPoint(localT, p[0], p[1], p[2], p[3]);
        }
        else
        {
            float localT = (t - 0.5f) * 2f; // scale 0.5–1 → 0–1
            pos = CalculateCubicBezierPoint(localT, p[4], p[5], p[6], p[7]);
        }

        transform.position = pos;

        // Update health bar UI
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }

    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0; // (1−t)^3 * P0
        point += 3 * uu * t * p1; // 3(1−t)^2 t * P1
        point += 3 * u * tt * p2; // 3(1−t)t^2 * P2
        point += ttt * p3;        // t^3 * P3
        return point;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
