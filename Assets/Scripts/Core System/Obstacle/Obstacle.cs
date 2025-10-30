using UnityEngine;

public class Obstacle : MonoBehaviour, IMovable
{
    public int laneIndex = 1;
    public float speed = 12f;
    public float destroyZ = -2f;
    public int damage = 5;
    public float hitRangeZ = 0.5f;

    private bool _hasHit = false;

    public void Move(float deltaTime)
    {
        Vector3 pos = transform.position;
        pos += Vector3.forward * (-speed * deltaTime);
        transform.position = pos;
    }

    // Update is called once per frame
    private void Update()
    {
        Move(Time.deltaTime);

        if (!_hasHit && PlayerController.Instance != null)
        {
            if (PlayerController.Instance.IsHitBy(transform.position, laneIndex))
            {
                PlayerController.Instance.TakeDamage(damage);
                _hasHit = true;
                Destroy(gameObject, 0.1f);
            }
        }

        if (transform.position.z <= destroyZ)
            Destroy(gameObject);
    }
}
