using System.Collections;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
    [Header("Appearance")]
    public Material shapeMaterial;
    public float spawnScaleDuration = 0.6f;
    public float eraseDuration = 0.6f;

    //--- Spawn platform and cube ---
    private ShapeMeshController platformController;
    private ShapeMeshController cubeController;

    void Start()
    {
        SpawnPlatform();
        SpawnCube();
    }

    void SpawnPlatform()
    {
        var generator = new PlatformGenerator();

        platformController = CreateShapeController(Vector3.zero, "Platform", null);

        Mesh mesh = generator.GenerateMesh();
        platformController.SetMeshInstant(mesh);
        platformController.SetMaterial(shapeMaterial);
        platformController.transform.localScale = Vector3.one;

        platformController.gameObject.AddComponent<PlatformPhysics>();
        platformController.physicsDriven = false;
    }

    void SpawnCube()
    {
        var generator = new CubeGenerator();

        cubeController = CreateShapeController(new Vector3(0, 5f, 0), "Cube", null);

        Mesh mesh = generator.GenerateMesh();
        cubeController.SetMeshInstant(mesh);
        cubeController.SetMaterial(shapeMaterial);
        cubeController.transform.localScale = Vector3.one;

        cubeController.physicsDriven = true;
        cubeController.gameObject.AddComponent<CubePhysicsController>();
    }

    private ShapeMeshController CreateShapeController(Vector3 position, string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.localPosition = position;

        ShapeMeshController controller = go.AddComponent<ShapeMeshController>();
        return controller;
    }
}
