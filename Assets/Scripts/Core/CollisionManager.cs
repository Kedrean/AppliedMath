using System.Collections.Generic;
using UnityEngine;

public class AABBBounds
{
    public Vector3 Center { get; private set; }
    public Vector3 Size { get; private set; }
    public Vector3 Extents { get; private set; }
    public Vector3 Min { get; private set; }
    public Vector3 Max { get; private set; }
    public int ID { get; private set; }
    public bool IsPlayer { get; private set; }
    public Matrix4x4 Matrix { get; set; }

    public AABBBounds(Vector3 center, Vector3 size, int id, bool isPlayer = false)
    {
        ID = id;
        IsPlayer = isPlayer;
        UpdateBounds(center, size);
    }

    public void UpdateBounds(Vector3 center, Vector3 size)
    {
        Center = center;
        Size = size;
        Extents = size * 0.5f; // Half-size for efficient calculations
        Min = center - Extents;
        Max = center + Extents;
    }

    public bool Intersects(AABBBounds other)
    {
        return !(Max.x < other.Min.x || Min.x > other.Max.x ||
                 Max.y < other.Min.y || Min.y > other.Max.y ||
                 Max.z < other.Min.z || Min.z > other.Max.z);
    }
}

public class CollisionManager : MonoBehaviour
{
    private static CollisionManager _instance;

    public static CollisionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("CollisionManager");
                _instance = go.AddComponent<CollisionManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private Dictionary<int, AABBBounds> _colliders = new Dictionary<int, AABBBounds>();
    private Dictionary<int, GameObject> _owners = new Dictionary<int, GameObject>();
    private int nextID = 0;

    /// Register a collider; returns an id. Owner is optional and can be set later with SetOwner.
    public int RegisterCollider(Vector3 center, Vector3 size, bool isPlayer = false)
    {
        int id = nextID++;
        _colliders[id] = new AABBBounds(center, size, id, isPlayer);
        Debug.Log($"Collider registered with ID {id}, center: {center}, size: {size}, isPlayer: {isPlayer}");
        return id;
    }

    public void SetOwner(int id, GameObject owner)
    {
        if (!_owners.ContainsKey(id))
            _owners.Add(id, owner);
        else
            _owners[id] = owner;
    }

    public GameObject GetOwner(int id)
    {
        if (_owners.TryGetValue(id, out GameObject go))
            return go;
        return null;
    }

    public void UpdateCollider(int id, Vector3 center, Vector3 size)
    {
        if (_colliders.TryGetValue(id, out AABBBounds bounds))
        {
            bounds.UpdateBounds(center, size);
        }
    }

    public void RemoveCollider(int id)
    {
        if (_colliders.ContainsKey(id))
        {
            _colliders.Remove(id);
            if (_owners.ContainsKey(id)) _owners.Remove(id);
        }
    }

    public void UpdateMatrix(int id, Matrix4x4 matrix)
    {
        if (_colliders.TryGetValue(id, out AABBBounds bounds))
        {
            bounds.Matrix = matrix;
            // keep center in sync for quick checks
            Vector3 pos = matrix.GetPosition();
            bounds.UpdateBounds(pos, bounds.Size);
        }
    }

    public bool CheckCollision(int id, Vector3 newCenter, out List<int> collidingIds)
    {
        collidingIds = new List<int>();
        if (!_colliders.TryGetValue(id, out AABBBounds current))
            return false;

        // Create a temporary bounds for collision check
        AABBBounds temp = new AABBBounds(newCenter, current.Size, -1);

        bool collided = false;
        foreach (var kvp in _colliders)
        {
            if (kvp.Key == id) continue; // Skip self-collision

            if (temp.Intersects(kvp.Value))
            {
                collidingIds.Add(kvp.Key);
                collided = true;
            }
        }
        return collided;
    }

    public Matrix4x4 GetMatrix(int id)
    {
        if (_colliders.TryGetValue(id, out AABBBounds bounds))
        {
            return bounds.Matrix;
        }
        return Matrix4x4.identity;
    }

    /// Expose a copy of all collider entries (id -> bounds) for tools like culling
    public Dictionary<int, AABBBounds> GetAllColliders()
    {
        return _colliders;
    }
}
