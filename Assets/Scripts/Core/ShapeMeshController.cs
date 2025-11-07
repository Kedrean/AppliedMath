using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ShapeMeshController : MonoBehaviour
{
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Coroutine morphRoutine;
    private Coroutine scaleRoutine;

    [Header("Rotation Settings")]
    public float rotationSpeed = 30f; // degrees per second

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshFilter.sharedMesh == null)
            meshFilter.sharedMesh = new Mesh();
    }

    private void Update()
    {
        float t = Time.time;
        float smoothSpeed = rotationSpeed * 0.5f;

        // Wobbling rotation for a smoother “alive” look
        transform.Rotate(Vector3.up, Mathf.Sin(t * 0.5f) * smoothSpeed * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.right, Mathf.Cos(t * 0.3f) * smoothSpeed * 0.6f * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.forward, Mathf.Sin(t * 0.4f + 1f) * smoothSpeed * 0.4f * Time.deltaTime, Space.World);
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
    }

    #region Morph Logic

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

    #endregion

    #region Scale Animation + Erase

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

    #endregion
}
