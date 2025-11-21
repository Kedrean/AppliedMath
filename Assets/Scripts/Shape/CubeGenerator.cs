using UnityEngine;

public class CubeGenerator : IShapeGenerator
{
    public string ShapeName => "Cube";

    public Mesh GenerateMesh(int segments = 0)
    {
        Mesh mesh = new Mesh();

        float size = 1f;
        float half = size * 0.5f;

        Vector3[] vertices = {
            new Vector3(-half, -half, -half),
            new Vector3( half, -half, -half),
            new Vector3( half,  half, -half),
            new Vector3(-half,  half, -half),

            new Vector3(-half, -half,  half),
            new Vector3( half, -half,  half),
            new Vector3( half,  half,  half),
            new Vector3(-half,  half,  half)
        };

        int[] triangles = {
            // back
            0, 2, 1, 0, 3, 2,
            // front
            4, 5, 6, 4, 6, 7,
            // left
            0, 4, 7, 0, 7, 3,
            // right
            1, 2, 6, 1, 6, 5,
            // top
            3, 7, 6, 3, 6, 2,
            // bottom
            0, 1, 5, 0, 5, 4
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
}
