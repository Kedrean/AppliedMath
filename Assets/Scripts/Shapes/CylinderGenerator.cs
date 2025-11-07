using UnityEngine;

public class CylinderGenerator : IShapeGenerator
{
    public string ShapeName => "Cylinder";

    public Mesh GenerateMesh(int segments = 32)
    {
        Mesh mesh = new Mesh();

        float height = 1f;
        float radius = 0.5f;
        float curveStrength = 0.15f;
        int rings = 10;
        float halfHeight = height * 0.5f;

        // --- Side Vertices ---
        int sideVertexCount = (segments + 1) * (rings + 1);
        Vector3[] sideVerts = new Vector3[sideVertexCount];

        for (int y = 0; y <= rings; y++)
        {
            float v = (float)y / rings;
            float yPos = v * height - halfHeight;

            float offset = (v - 0.5f) * 2f;
            float bulge = 1f - Mathf.Pow(Mathf.Abs(offset), 2f);
            float r = radius + bulge * curveStrength * 0.5f;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                float x = Mathf.Cos(angle) * r;
                float z = Mathf.Sin(angle) * r;
                sideVerts[y * (segments + 1) + i] = new Vector3(x, yPos, z);
            }
        }

        // --- Side Triangles ---
        int sideTriCount = segments * rings * 6;
        int[] sideTris = new int[sideTriCount];
        int t = 0;

        for (int y = 0; y < rings; y++)
        {
            for (int i = 0; i < segments; i++)
            {
                int a = y * (segments + 1) + i;
                int b = a + segments + 1;
                int c = a + 1;
                int d = b + 1;

                sideTris[t++] = a;
                sideTris[t++] = b;
                sideTris[t++] = c;
                sideTris[t++] = c;
                sideTris[t++] = b;
                sideTris[t++] = d;
            }
        }

        // --- Caps ---
        Vector3[] vertices = new Vector3[sideVerts.Length + (segments + 1) * 2 + 2];
        sideVerts.CopyTo(vertices, 0);
        int vIndex = sideVerts.Length;

        int bottomCenter = vIndex++;
        int topCenter = vIndex++;
        vertices[bottomCenter] = new Vector3(0, -halfHeight, 0);
        vertices[topCenter] = new Vector3(0, halfHeight, 0);

        int bottomStart = sideVerts.Length + 2;
        int topStart = bottomStart + (segments + 1);

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[bottomStart + i] = new Vector3(x, -halfHeight, z);
            vertices[topStart + i] = new Vector3(x, halfHeight, z);
        }

        int[] capTris = new int[segments * 6];
        int ct = 0;

        // bottom (reverse order)
        for (int i = 0; i < segments; i++)
        {
            capTris[ct++] = bottomCenter;
            capTris[ct++] = bottomStart + (i + 1) % (segments + 1);
            capTris[ct++] = bottomStart + i;
        }

        // top
        for (int i = 0; i < segments; i++)
        {
            capTris[ct++] = topCenter;
            capTris[ct++] = topStart + i;
            capTris[ct++] = topStart + (i + 1) % (segments + 1);
        }

        // --- Combine ---
        int[] triangles = new int[sideTris.Length + capTris.Length];
        sideTris.CopyTo(triangles, 0);
        capTris.CopyTo(triangles, sideTris.Length);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
}
