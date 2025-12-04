using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("References")]
    public EnhancedMeshGenerator meshGenerator;
    public Transform playerTransform;

    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject obstaclePrefab;
    public GameObject fireballPowerupPrefab;
    public GameObject goalPrefab;

    [Header("Spawn settings")]
    public float spawnStartX = 20f;      // spawn to the right of player
    public float spawnSpacing = 8f;
    public int spawnCount = 10;

    void Start()
    {
        if (meshGenerator == null) Debug.LogError("SpawnManager needs EnhancedMeshGenerator reference.");
        StartCoroutine(SpawnLinear());
    }

    IEnumerator SpawnLinear()
    {
        float x = playerTransform != null ? playerTransform.position.x + spawnStartX : spawnStartX;
        float fixedY = 1.5f;     // fixed Y spawn height
        float fixedZ = 1f;     // fixed Z spawn position

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = new Vector3(x, fixedY, fixedZ);

            float r = Random.value;

            if (r < 0.45f && enemyPrefab != null)
            {
                Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }
            else if (r < 0.8f && obstaclePrefab != null)
            {
                Quaternion rot = (Random.value > 0.5f) ? Quaternion.identity : Quaternion.Euler(0, 0, 90f);
                Instantiate(obstaclePrefab, spawnPos, rot);
            }
            else if (fireballPowerupPrefab != null)
            {
                // Fireball powerup rotated Z = -90, raised by 1.5f on Y
                Instantiate(fireballPowerupPrefab, spawnPos + Vector3.up * 1.5f, Quaternion.Euler(0, 0, -90f));
            }

            x += spawnSpacing + Random.Range(2f, 6f);

            yield return null;
        }

        // Spawn goal at X=99, fixed Y,Z, rotated Y=90 degrees
        if (goalPrefab != null)
        {
            Vector3 goalPos = new Vector3(99f, fixedY, fixedZ);
            Quaternion goalRot = Quaternion.Euler(0, 90f, 0);
            Instantiate(goalPrefab, goalPos, goalRot);
        }
    }
}
