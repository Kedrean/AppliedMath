using System.Collections;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
    [Header("UI + Morph Settings")]
    public Transform uiRoot;
    public AnimationCurve morphCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float morphDuration = 1.2f;

    [Header("Appearance")]
    public Material shapeMaterial;
    public float spawnScaleDuration = 0.6f;
    public float eraseDuration = 0.6f;

    public ShapeMeshController UIShapeController => uiShapeController;

    private ShapeMeshController worldShapeController;
    private ShapeMeshController uiShapeController;

    // --- Shape Generation Triggers (called from buttons) ---
    public void GeneratePyramid() => GenerateShape(new PyramidGenerator());
    public void GenerateCylinder() => GenerateShape(new CylinderGenerator());
    public void GenerateRectangularColumn() => GenerateShape(new RectColumnGenerator());
    public void GenerateSphere() => GenerateShape(new SphereGenerator());
    public void GenerateCapsule() => GenerateShape(new CapsuleGenerator());

    // --- Shape Creation ---
    private void GenerateShape(IShapeGenerator generator)
    {
        EraseShapes(false); // Erase old shape if exists, but don’t instantly recreate
        StartCoroutine(GenerateAfterDelay(generator));
    }

    private IEnumerator GenerateAfterDelay(IShapeGenerator generator)
    {
        yield return new WaitForSeconds(eraseDuration + 0.1f);
        CreateShapes(generator);
    }

    private void CreateShapes(IShapeGenerator generator)
    {
        worldShapeController = CreateShapeController(Vector3.zero, "WorldShape", null);
        uiShapeController = CreateShapeController(new Vector3(135, 0, -50), "UIShape", uiRoot);

        // Make world shape invisible initially
        SetWorldShapeVisible(false);

        Mesh mesh = generator.GenerateMesh(20);
        worldShapeController.SetMeshInstant(mesh);
        uiShapeController.SetMeshInstant(mesh);

        worldShapeController.ScaleIn(Vector3.one * 3.5f, spawnScaleDuration);
        uiShapeController.ScaleIn(Vector3.one * 50f, spawnScaleDuration);
    }

    private ShapeMeshController CreateShapeController(Vector3 position, string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.localPosition = position;

        ShapeMeshController controller = go.AddComponent<ShapeMeshController>();

        if (shapeMaterial != null)
            controller.SetMaterial(shapeMaterial);

        return controller;
    }

    // --- Erase and Cleanup ---
    public void EraseShapes(bool recreateAfter = true)
    {
        if (worldShapeController != null)
            worldShapeController.ScaleOutAndDestroy(eraseDuration);
        if (uiShapeController != null)
            uiShapeController.ScaleOutAndDestroy(eraseDuration);

        worldShapeController = null;
        uiShapeController = null;

        if (recreateAfter)
            Invoke(nameof(CreateDefaultShapes), eraseDuration + 0.1f);
    }

    private void CreateDefaultShapes()
    {
        CreateShapes(new SphereGenerator());
    }

    // --- Visibility Control (for toggling UI/world view) ---
    public void SetWorldShapeVisible(bool visible)
    {
        if (worldShapeController != null)
        {
            var renderer = worldShapeController.GetComponent<MeshRenderer>();
            if (renderer != null)
                renderer.enabled = visible;
        }
    }
}
