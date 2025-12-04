using System.Collections.Generic;
using UnityEngine;

public class OffscreenCuller : MonoBehaviour
{
    public Camera mainCamera;
    public float horizontalMargin = 40f; // how far beyond camera to keep active
    public float checkInterval = 0.5f;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        StartCoroutine(CullLoop());
    }

    System.Collections.IEnumerator CullLoop()
    {
        while (true)
        {
            CullOnce();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    void CullOnce()
    {
        if (mainCamera == null) return;
        float camX = mainCamera.transform.position.x;
        float leftLimit = camX - horizontalMargin;
        float rightLimit = camX + horizontalMargin;

        var colliders = CollisionManager.Instance.GetAllColliders();
        foreach (var kvp in colliders)
        {
            int id = kvp.Key;
            var bounds = kvp.Value;
            GameObject owner = CollisionManager.Instance.GetOwner(id);

            // skip player and ground if desired
            if (owner == null) continue;
            if (owner.GetComponent<PlayerController>() != null) continue;
            if (owner.GetComponent<EnhancedMeshGenerator>() != null) continue;

            Vector3 center = bounds.Center;
            if (center.x < leftLimit || center.x > rightLimit)
            {
                // offscreen -> set effective size to zero (disables collision)
                CollisionManager.Instance.UpdateCollider(id, center, Vector3.zero);

                // If the owner supports SetScaleZero, call it
                var instakill = owner.GetComponent<InstakillObstacle>();
                if (instakill != null) instakill.SetScaleZero();
            }
            else
            {
                // On-screen: ensure collider size is correct if we can reconstruct it from object's size
                // Many objects already update their collider in their own Update/Start, so we skip
            }
        }
    }
}
