using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ShapeMeshController : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Coroutine morphRoutine;
    private Coroutine scaleRoutine;

    public bool physicsDriven = false;

    private Mesh originalMesh;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshFilter.sharedMesh == null)
            meshFilter.sharedMesh = new Mesh();

        originalMesh = Instantiate(meshFilter.sharedMesh);
    }

    // --- Material ---
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

    // --- Mesh Set ---
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
                result[i] = Vector3.Lerp(normStartVerts[i], normEndVerts[i], t);

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

    // --- Scale Animations ---
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
}
