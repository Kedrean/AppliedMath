using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float baseSpawnInterval;
    [SerializeField] private float randomVariance;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            SpawnObstacle();
            float t = baseSpawnInterval + Random.Range(-randomVariance, randomVariance);
            yield return new WaitForSeconds(Mathf.Max(0.3f, t));
        }
    }

    private void SpawnObstacle()
    {
        int lane = Random.Range(0, 3);

        Vector3 start, end;
        float z = PlayerController.Instance.spawnZ;

        switch (lane)
        {
            case 0:
                start = new Vector3(-20f, 60f, z);
                end = new Vector3(-35f, -48f, z);
                break;
            case 1:
                start = new Vector3(0f, 60f, z);
                end = new Vector3(0f, -48f, z);
                break;
            default:
                start = new Vector3(20f, 60f, z);
                end = new Vector3(35f, -48f, z);
                break;
        }

        GameObject go = Instantiate(obstaclePrefab, start, Quaternion.identity);

        Obstacle prefabObstacle = obstaclePrefab.GetComponent<Obstacle>();
        Obstacle ob = go.GetComponent<Obstacle>();

        ob.laneIndex = lane;
        ob.startPos = start;
        ob.endPos = end;

        if (prefabObstacle != null)
            ob.speed = prefabObstacle.speed;
            ob.destructionMargin = prefabObstacle.destructionMargin;
    }
}
