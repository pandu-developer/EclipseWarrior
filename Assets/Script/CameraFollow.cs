using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // drag Player ke sini
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 2, -10);

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position, targetPos, smoothSpeed * Time.deltaTime
        );
    }
}