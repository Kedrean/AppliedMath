using UnityEngine;

public class Obstacle : MonoBehaviour, IMovable
{
    public int laneIndex;
    public float speed;
    public int damage;
    public float hitRangeY;
    public float hitRangeZ;
    public float destructionMargin;
    public Vector3 startScale = Vector3.one;
    public Vector3 endScale = Vector3.one;

    public Vector3 startPos { get; set; }
    public Vector3 endPos { get; set; }

    private bool _hasHit = false;
    private Vector3 _direction;

    private void Start()
    {
        transform.position = startPos;
        _direction = (endPos - startPos).normalized;

        // Flat rotation facing "down the lane"
        SetLaneRotation();
    }

    private void SetLaneRotation()
    {
        // Keep them flat (no pitch/roll) but rotated visually by lane
        switch (laneIndex)
        {
            case 0: // Left lane
                transform.rotation = Quaternion.Euler(0f, 0f, 260f);
                break;
            case 1: // Middle lane
                transform.rotation = Quaternion.Euler(0f, 0f, 270f);
                break;
            case 2: // Right lane
                transform.rotation = Quaternion.Euler(0f, 0f, 280f);
                break;
        }
    }

    public void Move(float deltaTime)
    {
        transform.position += _direction * speed * deltaTime;

        // Calculate travel progress (0 = start, 1 = end)
        float totalDistance = Vector3.Distance(startPos, endPos);
        float traveledDistance = Vector3.Distance(startPos, transform.position);
        float t = Mathf.Clamp01(traveledDistance / totalDistance);

        // Interpolate scale
        transform.localScale = Vector3.Lerp(startScale, endScale, t);
    }

    private void Update()
    {
        Move(Time.deltaTime);

        if (_hasHit || PlayerController.Instance == null)
            return;

        Vector3 playerPos = PlayerController.Instance.transform.position;
        bool sameLane = PlayerController.Instance.IsHitBy(transform.position, laneIndex);
        bool evading = PlayerController.Instance.IsEvading; // true if player is in air

        // Only damage if same lane, not evading, and close enough in Y
        float distanceY = Mathf.Abs(transform.position.y - playerPos.y);

        if (sameLane && !evading && distanceY <= hitRangeY)
        {
            PlayerController.Instance.TakeDamage(damage);
            _hasHit = true;
            Destroy(gameObject, 0.05f);
            return;
        }


        // Always destroy once it's below screen
        if (transform.position.y <= destructionMargin)
        {
            Destroy(gameObject);
        }
    }
}
