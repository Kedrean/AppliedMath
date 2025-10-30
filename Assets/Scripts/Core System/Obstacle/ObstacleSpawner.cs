using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float spawnZ = 40f;
    [SerializeField] private float baseSpawnInterval = 1f;
    [SerializeField] private float randomVariance = 0.5f;
    [SerializeField] private float obstacleSpeed = 12f;

    private float[] _lanes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        float offset = PlayerController.Instance.laneOffset;
        _lanes = new float[] { -offset, 0f, offset };
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            SpawnObstacle();
            float t = baseSpawnInterval + Random.Range(-randomVariance, randomVariance);
            yield return new WaitForSeconds(Mathf.Max(0.2f, t));
        }
    }

    private void SpawnObstacle()
    {
        int lane = Random.Range(0, 3);
        Vector3 pos = new Vector3(_lanes[lane], 0, spawnZ);
        GameObject go = Instantiate(obstaclePrefab, pos, Quaternion.identity);
        Obstacle ob = go.GetComponent<Obstacle>();
        ob.laneIndex = lane;
        ob.speed = obstacleSpeed;
    }
}
