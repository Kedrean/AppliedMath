using UnityEngine;

public interface IShapeGenerator
{
    Mesh GenerateMesh(int segments = 20);
    string ShapeName { get; }
}
