using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public int maxEnemies = 10;

    private float timer;
    private List<EnemyController> activeEnemies = new List<EnemyController>();

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f && activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }

        // Clean null references
        activeEnemies.RemoveAll(e => e == null);
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab);
        activeEnemies.Add(enemy.GetComponent<EnemyController>());
    }

    public List<EnemyController> GetActiveEnemies()
    {
        return activeEnemies;
    }
}
