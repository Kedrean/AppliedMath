using UnityEngine;

public class Control : MonoBehaviour
{
    public float moveSpeed = 3f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.position += new Vector3(h, v, 0) * moveSpeed * Time.deltaTime;
    }
}
