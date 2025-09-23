using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject increasePrefab;
    public GameObject decreasePrefab;

    public float spawnInterval = 5f;
    private float timer;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnPowerUp();
            timer = spawnInterval;
        }
    }

    void SpawnPowerUp()
    {
        // Pick random type
        GameObject prefab = (Random.value > 0.5f) ? increasePrefab : decreasePrefab;

        // Pick random position in camera space
        Vector2 spawnPos = GetRandomPointInCamera();

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    Vector2 GetRandomPointInCamera()
    {
        Camera cam = Camera.main;
        float x = Random.value; // 0..1
        float y = Random.value; // 0..1
        Vector3 worldPos = cam.ViewportToWorldPoint(new Vector3(x, y, cam.nearClipPlane));
        return new Vector2(worldPos.x, worldPos.y);
    }
}
