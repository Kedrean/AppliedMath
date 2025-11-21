using UnityEngine;

public class PlatformGenerator : IShapeGenerator
{
    public string ShapeName => "Platform";

    public Mesh GenerateMesh(int segments = 0)
    {
        Mesh mesh = new Mesh();

        float width = 6f;
        float depth = 2f;
        float height = 0.3f;
        float halfHeight = height * 0.5f;

        Vector3[] vertices = {
            new Vector3(-width, -halfHeight, -depth),
            new Vector3( width, -halfHeight, -depth),
            new Vector3( width, -halfHeight,  depth),
            new Vector3(-width, -halfHeight,  depth),

            new Vector3(-width,  halfHeight, -depth),
            new Vector3( width,  halfHeight, -depth),
            new Vector3( width,  halfHeight,  depth),
            new Vector3(-width,  halfHeight,  depth)
        };

        int[] triangles = {
            0, 2, 1, 0, 3, 2,
            4, 5, 6, 4, 6, 7,
            0, 1, 5, 0, 5, 4,
            1, 2, 6, 1, 6, 5,
            2, 3, 7, 2, 7, 6,
            3, 0, 4, 3, 4, 7
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
}
