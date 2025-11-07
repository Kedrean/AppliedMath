using UnityEngine;

public class CapsuleGenerator : IShapeGenerator
{
    public string ShapeName => "Capsule";

    public Mesh GenerateMesh(int segments = 16)
    {
        Mesh mesh = new Mesh();
        float radius = 0.5f;
        float height = 2f;
        float halfHeight = height * 0.5f;

        int hemisphereSegments = segments / 2;
        int rings = segments + 1;
        int points = rings * (segments + 1);

        Vector3[] vertices = new Vector3[points];
        int vert = 0;

        for (int y = 0; y <= segments; y++)
        {
            float v = (float)y / segments;
            float theta = v * Mathf.PI;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);

            for (int x = 0; x <= segments; x++)
            {
                float u = (float)x / segments;
                float phi = u * Mathf.PI * 2f;

                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                Vector3 vertex = new Vector3(
                    cosPhi * sinTheta * radius,
                    cosTheta * radius,
                    sinPhi * sinTheta * radius
                );

                if (y < hemisphereSegments)
                    vertex.y += halfHeight - radius;
                else if (y > hemisphereSegments)
                    vertex.y -= halfHeight - radius;

                vertices[vert++] = vertex;
            }
        }

        int[] triangles = new int[segments * segments * 6];
        int tri = 0;

        for (int y = 0; y < segments; y++)
        {
            for (int x = 0; x < segments; x++)
            {
                int i0 = y * (segments + 1) + x;
                int i1 = i0 + 1;
                int i2 = i0 + (segments + 1);
                int i3 = i2 + 1;

                triangles[tri++] = i0;
                triangles[tri++] = i2;
                triangles[tri++] = i1;
                triangles[tri++] = i1;
                triangles[tri++] = i2;
                triangles[tri++] = i3;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
