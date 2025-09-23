using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject increasePrefab;
    public GameObject decreasePrefab;

    public float spawnInterval = 5f;
    public float minDistance = 1.0f;      // minimum spacing between power-ups
    public float originBlockRadius = 2f;  // no spawns within this radius of (0,0)
    public int maxSpawnAttempts = 10;

    private float timer;
    private List<GameObject> activePowerUps = new List<GameObject>();

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            TrySpawnPowerUp();
            timer = spawnInterval;
        }

        // Cleanup destroyed ones
        activePowerUps.RemoveAll(p => p == null);
    }

    void TrySpawnPowerUp()
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            GameObject prefab = (Random.value > 0.5f) ? increasePrefab : decreasePrefab;
            Vector2 spawnPos = GetRandomPointInCamera();

            // Prevent spawns too close to origin
            if (spawnPos.magnitude < originBlockRadius)
                continue;

            // Prevent overlap with existing power-ups
            bool overlaps = false;
            foreach (GameObject existing in activePowerUps)
            {
                if (Vector2.Distance(spawnPos, existing.transform.position) < minDistance)
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                GameObject newPowerUp = Instantiate(prefab, spawnPos, Quaternion.identity);
                activePowerUps.Add(newPowerUp);
                return;
            }
        }

        // If we get here, no valid spot was found after attempts
    }

    Vector2 GetRandomPointInCamera()
    {
        Camera cam = Camera.main;
        float x = Random.value;
        float y = Random.value;
        Vector3 worldPos = cam.ViewportToWorldPoint(new Vector3(x, y, -cam.transform.position.z));
        return new Vector2(worldPos.x, worldPos.y);
    }
}
