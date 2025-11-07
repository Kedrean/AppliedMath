using UnityEngine;

public class SphereGenerator : IShapeGenerator
{
    public string ShapeName => "Sphere";

    public Mesh GenerateMesh(int segments = 20)
    {
        Mesh mesh = new Mesh();

        int latitudeSegments = segments;
        int longitudeSegments = segments;
        float radius = 0.5f;

        Vector3[] vertices = new Vector3[(latitudeSegments + 1) * (longitudeSegments + 1)];
        int[] triangles = new int[latitudeSegments * longitudeSegments * 6];

        int vert = 0;
        for (int y = 0; y <= latitudeSegments; y++)
        {
            float v = (float)y / latitudeSegments;
            float theta1 = v * Mathf.PI;
            for (int x = 0; x <= longitudeSegments; x++)
            {
                float u = (float)x / longitudeSegments;
                float theta2 = u * Mathf.PI * 2f;
                float sinTheta1 = Mathf.Sin(theta1);
                vertices[vert++] = new Vector3(
                    Mathf.Cos(theta2) * sinTheta1 * radius,
                    Mathf.Cos(theta1) * radius,
                    Mathf.Sin(theta2) * sinTheta1 * radius);
            }
        }

        int triIndex = 0;
        for (int y = 0; y < latitudeSegments; y++)
        {
            for (int x = 0; x < longitudeSegments; x++)
            {
                int i0 = y * (longitudeSegments + 1) + x;
                int i1 = i0 + 1;
                int i2 = i0 + (longitudeSegments + 1);
                int i3 = i2 + 1;

                triangles[triIndex++] = i0;
                triangles[triIndex++] = i2;
                triangles[triIndex++] = i1;
                triangles[triIndex++] = i1;
                triangles[triIndex++] = i2;
                triangles[triIndex++] = i3;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
