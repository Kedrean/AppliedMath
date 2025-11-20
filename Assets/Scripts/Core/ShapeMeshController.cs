using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ShapeMeshController : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Coroutine morphRoutine;
    private Coroutine scaleRoutine;

    [Header("Animation Settings")]
    public float rotationSpeed = 30f; // degrees per second
    public float bobAmount = 0.25f;   // vertical bob height
    public float bobSpeed = 1.5f;     // vertical bob speed

    private float spinAngle = 0f;     // current spin angle in degrees
    private Mesh originalMesh;        // cached original mesh for stable base verts

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshFilter.sharedMesh == null)
            meshFilter.sharedMesh = new Mesh();

        originalMesh = Instantiate(meshFilter.sharedMesh);
    }

    private void Update()
    {
        if (originalMesh == null || originalMesh.vertexCount == 0)
            return;

        // Update spin angle
        spinAngle += rotationSpeed * Time.deltaTime;

        // Calculate bobbing vertical offset
        float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        // Compose transformation matrix: translation then rotation
        Matrix4x4 translation = Matrix4x4.Translate(new Vector3(0, yOffset, 0));
        Matrix4x4 rotation = GLRotation.RotateY(spinAngle);
        Matrix4x4 transformMatrix = translation * rotation;

        // Apply transform to mesh vertices
        ApplyTransformToMesh(transformMatrix);
    }

    private void ApplyTransformToMesh(Matrix4x4 matrix)
    {
        Vector3[] baseVerts = originalMesh.vertices;
        Vector3[] transformedVerts = new Vector3[baseVerts.Length];

        for (int i = 0; i < baseVerts.Length; i++)
            transformedVerts[i] = matrix.MultiplyPoint3x4(baseVerts[i]);

        Mesh mesh = meshFilter.sharedMesh;
        mesh.vertices = transformedVerts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public void SetMaterial(Material mat)
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        if (mat != null)
        {
            meshRenderer.sharedMaterial = mat;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            meshRenderer.receiveShadows = true;
        }
    }

    public void SetMeshInstant(Mesh mesh)
    {
        if (mesh == null) return;

        meshFilter.sharedMesh = Instantiate(mesh);
        meshFilter.sharedMesh.RecalculateNormals();
        meshFilter.sharedMesh.RecalculateBounds();

        originalMesh = Instantiate(meshFilter.sharedMesh);
    }

    // --- Morph Logic ---
    public void MorphToMesh(Mesh targetMesh, AnimationCurve curve, float duration)
    {
        if (targetMesh == null)
            return;

        if (morphRoutine != null)
            StopCoroutine(morphRoutine);

        morphRoutine = StartCoroutine(MorphRoutine(targetMesh, curve, duration));
    }

    private IEnumerator MorphRoutine(Mesh targetMesh, AnimationCurve curve, float duration)
    {
        if (targetMesh == null || targetMesh.vertexCount == 0)
            yield break;

        Mesh startMesh = Instantiate(meshFilter.sharedMesh ?? new Mesh());
        Mesh workingMesh = new Mesh();
        meshFilter.sharedMesh = workingMesh;

        Vector3[] startVerts = startMesh.vertexCount > 0 ? startMesh.vertices : new Vector3[targetMesh.vertexCount];
        Vector3[] endVerts = targetMesh.vertices;
        Vector3[] normStartVerts = NormalizeVertexCount(startVerts, endVerts.Length);
        Vector3[] normEndVerts = NormalizeVertexCount(endVerts, normStartVerts.Length);
        int[] triangles = targetMesh.triangles;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = curve.Evaluate(time / duration);

            Vector3[] result = new Vector3[normEndVerts.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Vector3.Lerp(normStartVerts[i % normStartVerts.Length], normEndVerts[i % normEndVerts.Length], t);

            workingMesh.Clear();
            workingMesh.vertices = result;
            workingMesh.triangles = triangles;
            workingMesh.RecalculateNormals();
            workingMesh.RecalculateBounds();
            yield return null;
        }

        workingMesh.Clear();
        workingMesh.vertices = endVerts;
        workingMesh.triangles = triangles;
        workingMesh.RecalculateNormals();
        workingMesh.RecalculateBounds();
    }

    private Vector3[] NormalizeVertexCount(Vector3[] source, int targetLength)
    {
        if (source == null || source.Length == 0)
            return new Vector3[targetLength];
        if (source.Length == targetLength)
            return source;

        Vector3[] adjusted = new Vector3[targetLength];
        for (int i = 0; i < targetLength; i++)
            adjusted[i] = source[i % source.Length];
        return adjusted;
    }

    // --- Scale Animation + Erase ---
    public void ScaleIn(Vector3 targetScale, float duration)
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleRoutine(Vector3.zero, targetScale, duration));
    }

    public void ScaleOutAndDestroy(float duration)
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(SafeScaleAndDestroyRoutine(duration));
    }

    private IEnumerator ScaleRoutine(Vector3 from, Vector3 to, float duration)
    {
        transform.localScale = from;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(from, to, time / duration);
            yield return null;
        }

        transform.localScale = to;
    }

    private IEnumerator SafeScaleAndDestroyRoutine(float duration)
    {
        Vector3 from = transform.localScale;
        Vector3 to = Vector3.zero;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(from, to, time / duration);
            yield return null;
        }

        transform.localScale = to;
        Destroy(gameObject);
    }

    // --- GL Rotation Helper ---
    public static class GLRotation
    {
        public static Matrix4x4 RotateX(float degrees)
        {
            float r = degrees * Mathf.Deg2Rad;
            float c = Mathf.Cos(r);
            float s = Mathf.Sin(r);

            return new Matrix4x4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, c, -s, 0),
                new Vector4(0, s, c, 0),
                new Vector4(0, 0, 0, 1)
            );
        }

        public static Matrix4x4 RotateY(float degrees)
        {
            float r = degrees * Mathf.Deg2Rad;
            float c = Mathf.Cos(r);
            float s = Mathf.Sin(r);

            return new Matrix4x4(
                new Vector4(c, 0, s, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(-s, 0, c, 0),
                new Vector4(0, 0, 0, 1)
            );
        }

        public static Matrix4x4 RotateZ(float degrees)
        {
            float r = degrees * Mathf.Deg2Rad;
            float c = Mathf.Cos(r);
            float s = Mathf.Sin(r);

            return new Matrix4x4(
                new Vector4(c, -s, 0, 0),
                new Vector4(s, c, 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1)
            );
        }
    }
}
