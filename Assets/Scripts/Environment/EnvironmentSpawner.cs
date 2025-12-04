using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject cubePrefab;      // Assign your prefab here
    public int spawnCount = 50;

    public float minX = -50f;
    public float maxX = 50f;
    public float minY = 1f;  // Minimum height above ground
    public float maxY = 10f;
    public float constantZPosition = 0f;

    // Minimum distance between spawned cubes
    public float minSpawnDistance = 3f;

    void Start()
    {
        SpawnCubes();
    }

    void SpawnCubes()
    {
        List<Vector3> spawnedPositions = new List<Vector3>();
        List<Vector3> spawnedSizes = new List<Vector3>(); // to hold scaled sizes for accurate collision check

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos;
            Vector3 size;
            Quaternion rot;
            int tries = 0;
            bool validPosition = false;

            do
            {
                pos = new Vector3(
                    Random.Range(minX, maxX),
                    Random.Range(minY, maxY),
                    constantZPosition
                );

                // Random rotation on Z axis only (0°, 90°, 180°, 270°)
                float zRot = Random.Range(0, 4) * 90f;
                rot = Quaternion.Euler(0, 0, zRot);

                // Random scale
                float width = Random.Range(0.5f, 3f);
                float height = Random.Range(0.5f, 3f);
                float depth = Random.Range(0.5f, 3f);
                size = new Vector3(width, height, depth);

                // Check if this position overlaps with any existing cube, considering sizes
                validPosition = true;

                for (int j = 0; j < spawnedPositions.Count; j++)
                {
                    // Approximate check using bounding spheres:
                    float minDist = (spawnedSizes[j].magnitude + size.magnitude) * 0.5f + minSpawnDistance;

                    if (Vector3.Distance(pos, spawnedPositions[j]) < minDist)
                    {
                        validPosition = false;
                        break;
                    }
                }

                tries++;
                if (tries > 30) // Avoid infinite loops, allow some overlaps after many tries
                {
                    validPosition = true;
                }

            } while (!validPosition);

            GameObject cube = Instantiate(cubePrefab, pos, rot);
            cube.transform.localScale = size;

            // Register collider with CollisionManager
            var boxCollider = cube.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                Vector3 posRegistered = cube.transform.position;
                Vector3 sizeRegistered = Vector3.Scale(boxCollider.size, cube.transform.localScale);
                Quaternion rotRegistered = cube.transform.rotation;

                int id = CollisionManager.Instance.RegisterCollider(posRegistered, sizeRegistered, false);
                CollisionManager.Instance.UpdateMatrix(id, Matrix4x4.TRS(posRegistered, rotRegistered, Vector3.one));
            }

            spawnedPositions.Add(pos);
            spawnedSizes.Add(size);
        }
    }

}
