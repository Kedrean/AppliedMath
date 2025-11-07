using UnityEngine;

public class PyramidGenerator : IShapeGenerator
{
    public string ShapeName => "Pyramid";

    public Mesh GenerateMesh(int segments = 4)
    {
        Mesh mesh = new Mesh();
        float halfHeight = 0.5f;

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, -halfHeight, -0.5f),
            new Vector3( 0.5f, -halfHeight, -0.5f),
            new Vector3( 0.5f, -halfHeight,  0.5f),
            new Vector3(-0.5f, -halfHeight,  0.5f),
            new Vector3( 0, halfHeight, 0)
        };

        int[] triangles = new int[]
        {
            0, 1, 4,
            1, 2, 4,
            2, 3, 4,
            3, 0, 4,
            0, 3, 2,
            2, 1, 0
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
